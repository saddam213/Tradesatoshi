﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Base.Extensions;
using TradeSatoshi.Base.Queueing;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Faucet;
using TradeSatoshi.Common.Logging;
using TradeSatoshi.Common.Repositories.Trade;
using TradeSatoshi.Common.Services;
using TradeSatoshi.Common.Services.AuditService;
using TradeSatoshi.Common.Services.NotificationService;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Core.Logger;
using TradeSatoshi.Data;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Entity;
using TradeSatoshi.Enums;
using Tables = TradeSatoshi.Entity;

namespace TradeSatoshi.Core.Services
{
	public class TradeService : ITradeService
	{
		private static readonly ProcessorQueue<ITradeItem, ITradeResponse> TradeProcessor = new ProcessorQueue<ITradeItem, ITradeResponse>(TradeService.Processor());

		private static Func<ITradeItem, Task<ITradeResponse>> Processor()
		{
			var service = new TradeService();
			return service.ProcessTradeItem;
		}
		public ILogger Log { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IAuditService AuditService { get; set; }
		public ICacheService CacheService { get; set; }
		public INotificationService TradeNotificationService { get; set; }

		public TradeService()
		{
			DataContextFactory = new DataContextFactory();
			Log = new DatabaseLogger(DataContextFactory);
			AuditService = new AuditService(Log);
			CacheService = new CacheService();
			TradeNotificationService = new NotificationService();
		}

		public async Task<CreateTradeResponse> QueueTrade(CreateTradeModel tradeItem)
		{
			var result = await TradeProcessor.QueueItem(tradeItem).ConfigureAwait(false);
			return result as CreateTradeResponse;
		}

		public async Task<CancelTradeResponse> QueueCancel(CancelTradeModel tradeItem)
		{
			var result = await TradeProcessor.QueueItem(tradeItem).ConfigureAwait(false);
			return result as CancelTradeResponse;
		}

		public async Task<CreateTransferResponse> QueueTransfer(CreateTransferModel tradeItem)
		{
			var result = await TradeProcessor.QueueItem(tradeItem).ConfigureAwait(false);
			return result as CreateTransferResponse;
		}

		public async Task<CreateFaucetPaymentResponse> QueueFaucetPayment(CreateFaucetPaymentModel tradeItem)
		{
			var result = await TradeProcessor.QueueItem(tradeItem).ConfigureAwait(false);
			return result as CreateFaucetPaymentResponse;
		}

		private async Task<ITradeResponse> ProcessTradeItem(ITradeItem tradeItem)
		{
			var create = tradeItem as CreateTradeModel;
			if (create != null)
			{
				return await CreateTrade(create);
			}

			var cancel = tradeItem as CancelTradeModel;
			if (cancel != null)
			{
				return await CancelTrade(cancel);
			}

			var transfer = tradeItem as CreateTransferModel;
			if (transfer != null)
			{
				return await CreateTransfer(transfer);
			}

			var faucetPayment = tradeItem as CreateFaucetPaymentModel;
			if (faucetPayment != null)
			{
				return await CreateFaucetPayment(faucetPayment);
			}
			return null;
		}

		private async Task<ITradeResponse> CreateFaucetPayment(CreateFaucetPaymentModel faucetPayment)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{
						var faucetBot = await context.Users.FirstOrDefaultAsync(x => x.Id == Constants.SystemFaucetUserId);
						if (faucetBot == null || !faucetBot.IsTransferEnabled)
							throw new TradeException("Your transfers are currently disabled.");

						var recipient = await context.Users.FirstOrDefaultAsync(x => x.Id == faucetPayment.UserId);
						if (recipient == null || !recipient.IsTransferEnabled)
							throw new TradeException("Receiver does not have transfers enabled.");

						var currency = await context.Currency.FirstOrDefaultAsync(x => x.Id == faucetPayment.CurrencyId && x.IsFaucetEnabled);
						if (currency == null)
							throw new TradeException("Faucet not enabled for Currency.");

						// TODO: Check payment row
						var latPayment = DateTime.UtcNow.AddHours(-24);
						var faucetPayments = await context.FaucetPayments.Where(x => x.CurrencyId == currency.Id && x.Timestamp > latPayment).ToListAsync();
						if (faucetPayments.Sum(x => x.Amount) > currency.FaucetMax)
							throw new TradeException($"Maximum faucet payments for {currency.Symbol} have been reached today.");

						if (faucetPayments.Any(x => x.UserId == faucetPayment.UserId))
							throw new TradeException($"You have already claimed the {currency.Symbol} faucet payment today.");

						if (faucetPayments.Any(x => x.IPAddress == faucetPayment.IPAddress))
							throw new TradeException($"IPAddress {faucetPayment.IPAddress} has already claimed the {currency.Symbol} faucet payment today.");

						var auditResult = await AuditService.AuditUserCurrency(context, Constants.SystemFaucetUserId, faucetPayment.CurrencyId);
						if (!auditResult.Success)
							throw new Exception($"Failed to audit user balance.");

						if (currency.FaucetPayment <= 0)
							throw new TradeException("Invalid faucet payment set.");

						if (currency.FaucetPayment > auditResult.Avaliable)
							throw new TradeException($"The {currency.Symbol} faucet is empty.");

						var transfer = new TransferHistory
						{
							Amount = currency.FaucetPayment,
							Fee = 0,
							CurrencyId = faucetPayment.CurrencyId,
							Timestamp = DateTime.UtcNow,
							ToUserId = recipient.Id,
							TransferType = TransferType.Faucet,
							UserId = faucetBot.Id,
						};
						context.TransferHistory.Add(transfer);

						var payment = new Entity.FaucetPayment
						{
							CurrencyId = currency.Id,
							UserId = recipient.Id,
							Amount = currency.FaucetPayment,
							IPAddress = faucetPayment.IPAddress,
							Timestamp = DateTime.UtcNow
						};
						context.FaucetPayments.Add(payment);

						await context.SaveChangesAsync();


						var userNotifications = new List<NotifyUser>();
						var balanceNotifications = new List<NotifyBalanceUpdate>();
						var faucetBotAudit = await AuditService.AuditUserCurrency(context, faucetBot.Id, faucetPayment.CurrencyId);
						if (!faucetBotAudit.Success)
							throw new Exception($"Failed to audit Faucet balance.");

						var receiverAudit = await AuditService.AuditUserCurrency(context, recipient.Id, faucetPayment.CurrencyId);
						if (!receiverAudit.Success)
							throw new Exception($"Failed to audit recipients balance, Currency: {currency.Symbol}");

						balanceNotifications.Add(new NotifyBalanceUpdate
						{
							CurrencyId = receiverAudit.CurrencyId,
							Symbol = receiverAudit.Symbol,
							HeldForTrades = receiverAudit.HeldForTrades,
							PendingWithdraw = receiverAudit.PendingWithdraw,
							Total = receiverAudit.Total,
							Unconfirmed = receiverAudit.Unconfirmed,
							UserId = receiverAudit.UserId,
							Avaliable = receiverAudit.Avaliable
						});
						userNotifications.Add(new NotifyUser
						{
							UserId = recipient.Id,
							Type = NotificationType.Info,
							Title = "Faucet Payment",
							Message = $"Claimed {currency.FaucetPayment:F8} {currency.Symbol}"
						});

						transaction.Commit();

						await TradeNotificationService.SendNotification(userNotifications);
						await TradeNotificationService.SendBalanceUpdate(balanceNotifications);

						return new CreateFaucetPaymentResponse {  Message = $"Successfully claimed {currency.FaucetPayment:F8} {currency.Symbol} from faucet." };
					}
					catch (TradeException ex)
					{
						await Log.Warn("TradeService", "Rollback database Transaction");
						transaction.Rollback();
						await Log.Warn("TradeService", ex.Message);
						return new CreateFaucetPaymentResponse { Error = ex.Message };
					}
					catch (Exception ex)
					{
						await Log.Error("TradeService", "Rollback databaseTransaction");
						transaction.TryRollback();
						await Log.Exception("TradeService", ex);
					}
				}
			}
			return new CreateTransferResponse { Error = "Failed to create transfer." };
		}

		private async Task<ITradeResponse> CreateTransfer(CreateTransferModel tradeItem)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{
						var user = await context.Users.FirstOrDefaultAsync(x => x.Id == tradeItem.UserId);
						if (user == null || !user.IsTransferEnabled)
							throw new TradeException("Your transfers are currently disabled.");

						var toUser = await context.Users.FirstOrDefaultAsync(x => x.Id == tradeItem.ToUser);
						if (toUser == null || !toUser.IsTransferEnabled)
							throw new TradeException("Receiver does not have transfers enabled.");

						var auditResult = await AuditService.AuditUserCurrency(context, user.Id, tradeItem.CurrencyId);
						if (!auditResult.Success)
							throw new Exception($"Failed to audit user balance, Currency: {tradeItem.Symbol}");

						if (tradeItem.Amount > auditResult.Avaliable)
							throw new TradeException("Insufficient funds for transfer.");

						var transfer = new TransferHistory
						{
							Amount = Math.Max(0, tradeItem.Amount),
							Fee = 0,
							CurrencyId = tradeItem.CurrencyId,
							Timestamp = DateTime.UtcNow,
							ToUserId = toUser.Id,
							TransferType = TransferType.User,
							UserId = user.Id,
						};

						context.TransferHistory.Add(transfer);
						await context.SaveChangesAsync();


						var userNotifications = new List<NotifyUser>();
						var balanceNotifications = new List<NotifyBalanceUpdate>();
						var senderAudit = await AuditService.AuditUserCurrency(context, user.Id, tradeItem.CurrencyId);
						if (!senderAudit.Success)
							throw new Exception($"Failed to audit User balance, Currency: {tradeItem.Symbol}");
						balanceNotifications.Add(new NotifyBalanceUpdate
						{
							CurrencyId = senderAudit.CurrencyId,
							Symbol = senderAudit.Symbol,
							HeldForTrades = senderAudit.HeldForTrades,
							PendingWithdraw = senderAudit.PendingWithdraw,
							Total = senderAudit.Total,
							Unconfirmed = senderAudit.Unconfirmed,
							UserId = senderAudit.UserId,
							Avaliable = senderAudit.Avaliable
						});
						userNotifications.Add(new NotifyUser
						{
							UserId = user.Id,
							Type = NotificationType.Info,
							Title = "New Transfer",
							Message = $"Sent {transfer.Amount} {tradeItem.Symbol} to {toUser.UserName}"
						});

						var receiverAudit = await AuditService.AuditUserCurrency(context, toUser.Id, tradeItem.CurrencyId);
						if (!receiverAudit.Success)
							throw new Exception($"Failed to audit ToUser balance, Currency: {tradeItem.Symbol}");
						balanceNotifications.Add(new NotifyBalanceUpdate
						{
							CurrencyId = receiverAudit.CurrencyId,
							Symbol = receiverAudit.Symbol,
							HeldForTrades = receiverAudit.HeldForTrades,
							PendingWithdraw = receiverAudit.PendingWithdraw,
							Total = receiverAudit.Total,
							Unconfirmed = receiverAudit.Unconfirmed,
							UserId = receiverAudit.UserId,
							Avaliable = receiverAudit.Avaliable
						});
						userNotifications.Add(new NotifyUser
						{
							UserId = toUser.Id,
							Type = NotificationType.Info,
							Title = "New Transfer",
							Message = $"Received {transfer.Amount} {tradeItem.Symbol} from {senderAudit.UserName}"
						});

						transaction.Commit();

						await TradeNotificationService.SendNotification(userNotifications);
						await TradeNotificationService.SendBalanceUpdate(balanceNotifications);

						return new CreateTransferResponse();
					}
					catch (TradeException ex)
					{
						await Log.Warn("TradeService", "Rollback database Transaction");
						transaction.Rollback();
						await Log.Warn("TradeService", ex.Message);
						return new CreateTransferResponse { Error = ex.Message };
					}
					catch (Exception ex)
					{
						await Log.Error("TradeService", "Rollback databaseTransaction");
						transaction.TryRollback();
						await Log.Exception("TradeService", ex);
					}
				}
			}
			return new CreateTransferResponse { Error = "Failed to create transfer." };
		}

		private async Task<CancelTradeResponse> CancelTrade(CancelTradeModel tradeItem)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{
						var response = new CancelTradeResponse();
						var audits = new HashSet<int>();
						var notifications = new List<INotify>();
						var user = await context.Users.FirstOrDefaultAsync(x => x.Id == tradeItem.UserId);
						if (user == null || !user.IsTradeEnabled)
							throw new TradeException("Your trades are currently disabled.");

						var cancelMessage = "Successfully canceled all orders";
						var orderQuery = context.Trade
							.Include(x => x.TradePair)
							.Where(x => x.UserId == user.Id && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending));
						switch (tradeItem.CancelType)
						{
							case TradeCancelType.Single:
								orderQuery = orderQuery.Where(x => x.Id == tradeItem.OrderId.Value);
								cancelMessage = $"Successfully canceled order #{tradeItem.OrderId.Value}";
								break;
							case TradeCancelType.Market:
								orderQuery = orderQuery.Where(x => x.TradePair.Name == tradeItem.Market);
								cancelMessage = $"Successfully canceled {tradeItem.Market} orders";
								break;
							case TradeCancelType.MarketBuys:
								orderQuery = orderQuery.Where(x => x.TradePair.Name == tradeItem.Market && x.TradeType == TradeType.Buy);
								cancelMessage = $"Successfully canceled {tradeItem.Market} buy orders";
								break;
							case TradeCancelType.MarketSells:
								orderQuery = orderQuery.Where(x => x.TradePair.Name == tradeItem.Market && x.TradeType == TradeType.Sell);
								cancelMessage = $"Successfully canceled {tradeItem.Market} sell orders";
								break;
							case TradeCancelType.AllBuys:
								orderQuery = orderQuery.Where(x => x.TradeType == TradeType.Buy);
								cancelMessage = "Successfully canceled all buy orders";
								break;
							case TradeCancelType.AllSells:
								orderQuery = orderQuery.Where(x => x.TradeType == TradeType.Sell);
								cancelMessage = "Successfully canceled all sell orders";
								break;
							default:
								break;
						}

						notifications.Add(new NotifyUser
						{
							UserId = user.Id,
							Type = NotificationType.Success,
							Title = "Trade Cancel",
							Message = cancelMessage
						});

						//var orderNotifications = new List<NotifyOrderBookUpdate>();
						//var openOrderNotifications = new List<NotifyOpenOrderUserUpdate>();
						var orders = await orderQuery.ToListAsync();
						foreach (var order in orders)
						{
							audits.Add(order.TradePair.CurrencyId1);
							audits.Add(order.TradePair.CurrencyId2);
							order.Status = TradeStatus.Canceled;
							response.AddCanceledOrder(order.Id);

							// Notify orderbook of open order cancel
							notifications.Add(new NotifyOrderBookUpdate
							{
								Action = "Cancel",
								Type = order.TradeType.ToString(),
								Market = order.TradePair.Name,
								TradePairId = order.TradePairId,
								Amount = order.Remaining,
								Price = order.Rate
							});

							// Notify user of open order cancel
							notifications.Add(new NotifyOpenOrderUserUpdate
							{
								Action = "Cancel",
								UserId = user.Id,
								OrderId = order.Id,
								TradePairId = order.TradePairId
							});
						}

						// Save changes
						await context.SaveChangesAsync();

						//var balanceNotifications = new List<NotifyBalanceUpdate>();
						//Audit the currency associated with the canceled trade
						foreach (var auditCurrency in audits)
						{
							var auditResult = await AuditService.AuditUserCurrency(context, user.Id, auditCurrency);
							if (!auditResult.Success)
								throw new Exception("Failed to audit user balance.");

							notifications.Add(new NotifyBalanceUpdate
							{
								CurrencyId = auditResult.CurrencyId,
								Symbol = auditResult.Symbol,
								HeldForTrades = auditResult.HeldForTrades,
								PendingWithdraw = auditResult.PendingWithdraw,
								Total = auditResult.Total,
								Unconfirmed = auditResult.Unconfirmed,
								UserId = auditResult.UserId,
								Avaliable = auditResult.Avaliable,
							});
						}

						transaction.Commit();

						// Send notification ans invalidate cache
						await TradeNotificationService.SendNotificationCollection(notifications);
						foreach (var tradePairId in orders.Select(x => x.TradePairId).Distinct())
						{
							CacheService.Invalidate(TradeCacheKeys.GetOpenBuyOrdersKey(tradePairId));
							CacheService.Invalidate(TradeCacheKeys.GetOpenSellOrdersKey(tradePairId));
						}

						return response;
					}
					catch (Exception ex)
					{
						await Log.Error("TradeService", "Rollback databaseTransaction");
						transaction.TryRollback();
						await Log.Exception("TradeService", ex);
					}
				}
			}
			return new CancelTradeResponse("Failed to cancel order.");
		}


		private async Task<ITradeResponse> CreateTrade(CreateTradeModel tradeRequest)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{
						await Log.Info("TradeService", $"Processing trade request. UserId: {tradeRequest.UserId}, TradeType: {tradeRequest.TradeType}, TradePairId: {tradeRequest.TradePairId}, Amount: {tradeRequest.Amount:F8}, Rate: {tradeRequest.Rate:F8}");

						var notifications = new List<INotify>();
						var response = new CreateTradeResponse();

						var tradeType = tradeRequest.TradeType;
						var user = await context.Users.FirstOrDefaultAsync(x => x.Id == tradeRequest.UserId);
						if (user == null || !user.IsTradeEnabled)
							throw new TradeException("Your trades are currently disabled.");

						// Get or cache tradepair
						var tradePair = await context.TradePair
							.Include(t => t.Currency1)
							.Include(t => t.Currency2)
							.FirstOrDefaultAsync(x => x.Id == tradeRequest.TradePairId || x.Name == tradeRequest.Market);
						if (tradePair == null)
							throw new Exception("Market not found.");

						if (tradePair.Status != TradePairStatus.OK)
						{
							throw new Exception($"Market status is currently {tradePair.Status}, unable to process trade.");
						}

						// Get or cache currency
						var currency = tradePair.Currency1;
						var baseCurency = tradePair.Currency2;

						int tradeCurrency = tradeRequest.TradeType == TradeType.Buy ? tradePair.CurrencyId2 : tradePair.CurrencyId1;
						decimal tradeRate = Math.Round(tradeRequest.Rate, 8);
						decimal tradeAmount = Math.Round(tradeRequest.Amount, 8);
						decimal tradeTotal = tradeRequest.TradeType == TradeType.Buy ? (tradeAmount * tradeRate).IncludingFees(baseCurency.TradeFee) : tradeAmount;
						decimal totalTradeAmount = tradeAmount * tradeRate;
						if (totalTradeAmount <= baseCurency.MinBaseTrade)
							throw new TradeException($"Invalid trade amount, Minumim total trade is {baseCurency.MinBaseTrade} {baseCurency.Symbol}.");

						if (tradeAmount >= 1000000000 || totalTradeAmount >= 1000000000)
							throw new TradeException("Invalid trade amount, Maximum single trade is 1000000000 coins");

						if (tradeRate < 0.00000001m)
							throw new TradeException($"Invalid trade price, minimum price is 0.00000001 {baseCurency.Symbol}");

						if (tradeRate > 1000000000)
							throw new TradeException($"Invalid trade price, maximum price is 1000000000 {baseCurency.Symbol}");

						//Audit the user balance for currency
						var auditResult = await AuditService.AuditUserCurrency(context, tradeRequest.UserId, tradeCurrency);
						if (!auditResult.Success || auditResult.Avaliable < tradeTotal)
							throw new TradeException("Insufficient funds for trade.");

						// Get existing trades that can be filled
						List<Tables.Trade> trades;
						if (tradeType == TradeType.Buy)
						{
							// Fetch any trades that can be filled for this request
							trades = await context.Trade
								.Where(o => o.TradePairId == tradePair.Id && (o.Status == TradeStatus.Pending || o.Status == TradeStatus.Partial) && o.TradeType == TradeType.Sell && o.Rate <= tradeRate)
								.OrderBy(o => o.Rate)
								.ThenBy(o => o.Timestamp)
								.ToListAsync();
						}
						else
						{
							// Fetch any trades that can be filled for this request
							trades = await context.Trade
								.Where(o => o.TradePairId == tradePair.Id && (o.Status == TradeStatus.Pending || o.Status == TradeStatus.Partial) && o.TradeType == TradeType.Buy && o.Rate >= tradeRate)
								.OrderByDescending(o => o.Rate)
								.ThenBy(o => o.Timestamp)
								.ToListAsync();
						}

						if (trades.IsNullOrEmpty())
						{
							// There are no trades to fill, so create trade
							var trade = CreateTrade(context.Trade, tradeType, tradeRequest.UserId, tradePair, tradeAmount, tradeRate, baseCurency.TradeFee, tradeRequest.IsApi);
							await Log.Debug("TradeService", "Submitting context changes...");
							await context.SaveChangesAsync();

							var userAudit = await AuditService.AuditUserTradePair(context, tradeRequest.UserId, tradePair);
							if (!userAudit.Success)
								throw new Exception("Failed to audit user balances.");

							response.TradeId = trade.Id;

							// notify user of balance change
							notifications.Add(new NotifyBalanceUpdate
							{
								CurrencyId = userAudit.Currency.CurrencyId,
								Symbol = userAudit.Currency.Symbol,
								HeldForTrades = userAudit.Currency.HeldForTrades,
								PendingWithdraw = userAudit.Currency.PendingWithdraw,
								Total = userAudit.Currency.Total,
								Unconfirmed = userAudit.Currency.Unconfirmed,
								UserId = userAudit.Currency.UserId,
								Avaliable = userAudit.Currency.Avaliable
							});
							notifications.Add(new NotifyBalanceUpdate
							{
								CurrencyId = userAudit.BaseCurrency.CurrencyId,
								Symbol = userAudit.BaseCurrency.Symbol,
								HeldForTrades = userAudit.BaseCurrency.HeldForTrades,
								PendingWithdraw = userAudit.BaseCurrency.PendingWithdraw,
								Total = userAudit.BaseCurrency.Total,
								Unconfirmed = userAudit.BaseCurrency.Unconfirmed,
								UserId = userAudit.BaseCurrency.UserId,
								Avaliable = userAudit.BaseCurrency.Avaliable
							});

							// notify user of order
							notifications.Add(new NotifyUser
							{
								Type = NotificationType.Info,
								UserId = tradeRequest.UserId,
								Title = "Order Placed",
								Message = $"{tradeType} {tradeAmount:F8} {currency.Symbol} @ {tradeRate:F8} {baseCurency.Symbol}",
							});

							// Notify order book of new order
							notifications.Add(new NotifyOrderBookUpdate
							{
								Action = "New",
								Type = trade.TradeType.ToString(),
								Market = tradePair.Name,
								TradePairId = tradePair.Id,
								Amount = trade.Remaining,
								Price = trade.Rate
							});

							// Notify user of new open order
							notifications.Add(new NotifyOpenOrderUserUpdate
							{
								Action = "New",
								UserId = user.Id,
								OrderId = trade.Id,
								Amount = trade.Amount,
								Market = tradePair.Name,
								TradePairId = tradePair.Id,
								Price = trade.Rate,
								Remaining = trade.Remaining,
								Type = trade.TradeType.ToString(),
								Timestamp = trade.Timestamp
							});
						}
						else
						{
							var audits = new HashSet<string> { user.Id };
							int tradesLimit = 20000;
							decimal buyersRefund = 0m;
							decimal tradeRemaining = tradeAmount;
							Tables.Trade remainingTrade = null;
							var newTransactions = new List<TradeHistory>();
							foreach (var trade in trades)
							{
								// If we are all done bail out
								if (Math.Round(tradeRemaining, 8) == 0 || tradesLimit == 0)
								{
									break;
								}

								// Can we fill this buy trade, or is it a partial
								var canFillTrade = trade.Remaining <= tradeRemaining;

								decimal dogeAmount = canFillTrade ? trade.Remaining : tradeRemaining;
								decimal actualBtcAmount = canFillTrade ? trade.Remaining * trade.Rate : tradeRemaining * trade.Rate;
								decimal expectedBtcAmount = canFillTrade ? trade.Remaining * tradeRate : tradeRemaining * tradeRate;
								if (tradeType == TradeType.Buy)
								{
									// Create transaction for the buy
									newTransactions.Add(CreateTransaction(context.TradeHistory, TradeType.Buy, tradeRequest.UserId, trade.UserId, tradePair, dogeAmount, trade.Rate, trade.Fee > 0 ? baseCurency.TradeFee : 0, tradeRequest.IsApi));

									// If the rate is cheaper than the buyers request, sort out a refund amount for notification
									buyersRefund += (expectedBtcAmount - actualBtcAmount);
								}
								else
								{
									// Create transaction for this fill
									newTransactions.Add(CreateTransaction(context.TradeHistory, TradeType.Sell, trade.UserId, tradeRequest.UserId, tradePair, dogeAmount, trade.Rate, trade.Fee > 0 ? baseCurency.TradeFee : 0, tradeRequest.IsApi));
								}

								if (canFillTrade)
								{
									// We can fill this whole trade so zero out and mark as 'Complete'
									trade.Remaining = 0;
									trade.Fee = 0;
									trade.Status = TradeStatus.Complete;
									tradeRemaining -= dogeAmount;
									await Log.Debug("TradeService", $"Filled TradeId: {trade.Id}");

									// Notify user orders of filled order
									notifications.Add(new NotifyOpenOrderUserUpdate
									{
										Action = "Fill",
										UserId = trade.UserId,
										OrderId = trade.Id,
										TradePairId = tradePair.Id,
									});

									// Notify orderbook of filled order
									notifications.Add(new NotifyOrderBookUpdate
									{
										Action = "Fill",
										Type = trade.TradeType.ToString(),
										Market = tradePair.Name,
										TradePairId = tradePair.Id,
										Amount = trade.Amount,
										Price = trade.Rate
									});
								}
								else
								{
									// We can only fill some of this trade, subtract the amount and mark at 'Partial'
									var lastTradeAmount = tradeRemaining;
									trade.Remaining -= tradeRemaining;
									trade.Fee = (trade.Remaining * trade.Rate).GetFees(trade.Fee > 0 ? baseCurency.TradeFee : 0);
									trade.Status = TradeStatus.Partial;
									tradeRemaining = 0;
									await Log.Debug("TradeService", $"Partially filled TradeId: {trade.Id}");

									//BUGFIX: if the remaining rounds out to 0.00000000 the trade is complete not partial
									if (Math.Round(trade.Remaining, 8) == 0)
									{
										trade.Remaining = 0;
										trade.Fee = 0;
										trade.Status = TradeStatus.Complete;
										await Log.Debug("TradeService", $"Partially filled resulted in 0.00000000 remaining, Filling TradeId: {trade.Id}");
									}

									// Notify user orders of partial filled order
									notifications.Add(new NotifyOpenOrderUserUpdate
									{
										Action = "Partial",
										UserId = trade.UserId,
										OrderId = trade.Id,
										TradePairId = tradePair.Id,
										Remaining = trade.Remaining,
									});

									// Notify orderbook of partial filled order
									notifications.Add(new NotifyOrderBookUpdate
									{
										Action = "Partial",
										Type = trade.TradeType.ToString(),
										Market = tradePair.Name,
										TradePairId = tradePair.Id,
										Amount = lastTradeAmount,
										Price = trade.Rate
									});
								}

								tradePair.LastTrade = trade.Rate;

								// Add to Audit list
								audits.Add(trade.UserId);

								// Notify buyer
								notifications.Add(new NotifyUser
								{
									Type = NotificationType.Info,
									UserId = tradeRequest.UserId,
									Title = "Order Filled",
									Message = $"You {(tradeType == TradeType.Buy ? "bought" : "sold")} {dogeAmount:F8} {currency.Symbol} for {actualBtcAmount:F8} {baseCurency.Symbol}",
								});

								// Notify seller
								notifications.Add(new NotifyUser
								{
									Type = NotificationType.Info,
									UserId = trade.UserId,
									Title = "Order Filled",
									Message = $"You {(tradeType == TradeType.Buy ? "Sold" : "bought")} {dogeAmount:F8} {currency.Symbol} for {actualBtcAmount:F8} {baseCurency.Symbol}",
								});

								// Update trade limiter
								tradesLimit--;
							}

							// Update tradepair stats
							var hours = DateTime.UtcNow.AddHours(-24);
							var lastTrade = await context.TradeHistory
								.Where(x => x.TradePairId == tradePair.Id && x.Timestamp >= hours)
								.OrderBy(x => x.Id)
								.FirstOrDefaultAsync();
							tradePair.Change = GetChangePercent(lastTrade?.Rate ?? 0, tradePair.LastTrade);

							// If the remaining is not 0 create an trade for the rest
							if (Math.Round(tradeRemaining, 8) > 0 && tradesLimit > 0)
							{
								// create trade for remaining
								remainingTrade = CreateTrade(context.Trade, tradeType, tradeRequest.UserId, tradePair, tradeRemaining, tradeRate, baseCurency.TradeFee, tradeRequest.IsApi);
								await Log.Debug("TradeService", $"Created new trade, TradeId: {remainingTrade.Id}, TradeType: {remainingTrade.TradeType}, Amount: {remainingTrade.Amount}, Rate: {remainingTrade.Rate}");
								// notify user of order
								notifications.Add(new NotifyUser
								{
									Type = NotificationType.Info,
									UserId = tradeRequest.UserId,
									Title = "Order Placed",
									Message = $"{tradeType} {remainingTrade.Remaining:F8} {currency.Symbol} @ {tradeRate:F8} {baseCurency.Symbol}",
								});

								// Notify orderbook of new order
								notifications.Add(new NotifyOrderBookUpdate
								{
									Action = "New",
									Type = remainingTrade.TradeType.ToString(),
									Market = tradePair.Name,
									TradePairId = tradePair.Id,
									Amount = remainingTrade.Remaining,
									Price = remainingTrade.Rate
								});

								// Notify user of new open order
								notifications.Add(new NotifyOpenOrderUserUpdate
								{
									Action = "New",
									UserId = user.Id,
									OrderId = remainingTrade.Id,
									Amount = remainingTrade.Amount,
									Market = tradePair.Name,
									TradePairId = tradePair.Id,
									Price = remainingTrade.Rate,
									Remaining = remainingTrade.Remaining,
									Type = remainingTrade.TradeType.ToString(),
									Timestamp = remainingTrade.Timestamp
								});
							}

							// If there is any change, notify user of refund
							if (tradeType == TradeType.Buy && buyersRefund > 0)
							{
								notifications.Add(new NotifyUser
								{
									Type = NotificationType.Info,
									UserId = tradeRequest.UserId,
									Title = "Trade Refund",
									Message = $"Refunded {buyersRefund:F8} {baseCurency.Symbol} from buy trade",
								});
							}

							await Log.Debug("TradeService", "Submitting context changes...");
							await context.SaveChangesAsync();

							// Audit all users involved
							foreach (var userAudit in audits)
							{
								var userAuditResult = await AuditService.AuditUserTradePair(context, userAudit, tradePair);
								if (!userAuditResult.Success)
								{
									throw new Exception("Failed to audit user balances.");
								}

								// notify user of balance change
								notifications.Add(new NotifyBalanceUpdate
								{
									CurrencyId = userAuditResult.Currency.CurrencyId,
									Symbol = userAuditResult.Currency.Symbol,
									HeldForTrades = userAuditResult.Currency.HeldForTrades,
									PendingWithdraw = userAuditResult.Currency.PendingWithdraw,
									Total = userAuditResult.Currency.Total,
									Unconfirmed = userAuditResult.Currency.Unconfirmed,
									UserId = userAuditResult.Currency.UserId,
									Avaliable = userAuditResult.Currency.Avaliable
								});
								notifications.Add(new NotifyBalanceUpdate
								{
									CurrencyId = userAuditResult.BaseCurrency.CurrencyId,
									Symbol = userAuditResult.BaseCurrency.Symbol,
									HeldForTrades = userAuditResult.BaseCurrency.HeldForTrades,
									PendingWithdraw = userAuditResult.BaseCurrency.PendingWithdraw,
									Total = userAuditResult.BaseCurrency.Total,
									Unconfirmed = userAuditResult.BaseCurrency.Unconfirmed,
									UserId = userAuditResult.BaseCurrency.UserId,
									Avaliable = userAuditResult.BaseCurrency.Avaliable
								});
							}

							//Add filled and new order ids to response
							if (remainingTrade != null)
							{
								response.TradeId = remainingTrade.Id;
							}
							foreach (var newTransaction in newTransactions)
							{
								if (newTransaction != null)
									response.AddFilledTrade(newTransaction.Id);

								await Log.Debug("TradeService", $"Created new transaction, CurrencyId: {newTransaction.CurrencyId}, TransactionType: {newTransaction.TradeHistoryType}, Amount: {newTransaction.Amount}, Rate: {newTransaction.Rate}, Fee: {newTransaction.Fee}");
								// Notify trade history
								notifications.Add(new NotifyTradeHistoryUpdate
								{
									Amount = newTransaction.Amount,
									Market = tradePair.Name,
									TradePairId = tradePair.Id,
									Price = newTransaction.Rate,
									Type = newTransaction.TradeHistoryType.ToString(),
									Timestamp = newTransaction.Timestamp
								});

								// Notify user trade history
								notifications.Add(new NotifyTradeUserHistoryUpdate
								{
									UserId = newTransaction.UserId,
									Amount = newTransaction.Amount,
									Market = tradePair.Name,
									TradePairId = tradePair.Id,
									Price = newTransaction.Rate,
									Type = newTransaction.TradeHistoryType.ToString(),
									Timestamp = newTransaction.Timestamp
								});
								notifications.Add(new NotifyTradeUserHistoryUpdate
								{
									UserId = newTransaction.ToUserId,
									Amount = newTransaction.Amount,
									Market = tradePair.Name,
									TradePairId = tradePair.Id,
									Price = newTransaction.Rate,
									Type = newTransaction.TradeHistoryType.ToString(),
									Timestamp = newTransaction.Timestamp
								});
							}
						}

						await Log.Debug("TradeService", "Committing database transaction");
						await context.SaveChangesAsync();
						transaction.Commit();

						// Send notifications and invalidate cache
						await TradeNotificationService.SendNotificationCollection(notifications);
						CacheService.Invalidate(TradeCacheKeys.GetTradeHistoryKey(tradePair.Id));
						CacheService.Invalidate(TradeCacheKeys.GetOpenBuyOrdersKey(tradePair.Id));
						CacheService.Invalidate(TradeCacheKeys.GetOpenSellOrdersKey(tradePair.Id));

						await Log.Info("TradeService", "Process trade success.");
						return response;
					}
					catch (TradeException ex)
					{
						await Log.Warn("TradeService", "Rollback database Transaction");
						transaction.TryRollback();
						await Log.Warn("TradeService", ex.Message);

						// Notify user
						await TradeNotificationService.SendNotification(new NotifyUser
						{
							Type = NotificationType.Error,
							UserId = tradeRequest.UserId,
							Title = "Order Failed",
							Message = ex.Message,
						});

					}
					catch (Exception ex)
					{
						await Log.Error("TradeService", "Rollback database Transaction");
						transaction.TryRollback();
						await Log.Exception("TradeService", ex);

						// Notify user
						await TradeNotificationService.SendNotification(new NotifyUser
						{
							Type = NotificationType.Error,
							UserId = tradeRequest.UserId,
							Title = "Order Failed",
							Message = "An unknown error occured.",
						});
					}
				}
			}

			return new CreateTradeResponse { Error = "An unknown error occured" };
		}

		#region Helpers

		private Tables.Trade CreateTrade(DbSet<Tables.Trade> tradeTable, TradeType type, string userId, Entity.TradePair tradePair, decimal amount, decimal rate, decimal fee, bool isapi)
		{
			var newTrade = new Tables.Trade
			{
				TradePairId = tradePair.Id,
				Amount = Math.Round(amount, 8),
				Rate = Math.Round(rate, 8),
				Remaining = Math.Round(amount, 8),
				Status = TradeStatus.Pending,
				TradeType = type,
				UserId = userId,
				Timestamp = DateTime.UtcNow,
				Fee = (amount * rate).GetFees(fee),
				IsApi = isapi
			};
			tradeTable.Add(newTrade);
			return newTrade;
		}

		private TradeHistory CreateTransaction(DbSet<TradeHistory> transactionTable, TradeType type, string userId, string toUserId, Entity.TradePair tradepair, decimal amount, decimal rate, decimal fee, bool isapi)
		{
			var transaction = new TradeHistory
			{
				TradePairId = tradepair.Id,
				CurrencyId = tradepair.CurrencyId1,
				Amount = Math.Round(amount, 8),
				Rate = Math.Round(rate, 8),
				TradeHistoryType = type,
				UserId = userId,
				ToUserId = toUserId,
				Timestamp = DateTime.UtcNow,
				Fee = (amount * rate).GetFees(fee),
				IsApi = isapi
			};
			transactionTable.Add(transaction);
			return transaction;
		}

		private double GetChangePercent(decimal lastTrade, decimal newTrade)
		{
			if (lastTrade > 0)
			{
				return Math.Round((double)((newTrade - lastTrade) / lastTrade * 100m), 2);
			}
			return 0.00;
		}



		#endregion
	}

	[Serializable]
	public class TradeException : Exception
	{
		public TradeException(string message) : base(message)
		{
		}
	}
}