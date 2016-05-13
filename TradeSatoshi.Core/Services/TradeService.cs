using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Base.Extensions;
using TradeSatoshi.Base.Queueing;
using TradeSatoshi.Common.Data;
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
			AuditService = new AuditService();
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
			return null;
		}

		private async Task<ITradeResponse> CreateTransfer(CreateTransferModel tradeItem)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{
						var auditResult = await AuditService.AuditUserCurrency(context, tradeItem.UserId, tradeItem.CurrencyId);
						if (!auditResult.Success)
							throw new Exception($"Failed to audit user balance, Currency: {tradeItem.Symbol}");

						if (tradeItem.Amount > auditResult.Avaliable)
							throw new TradeException("Insufficient funds for transfer.");

						var toUser = await context.Users.FirstOrDefaultAsync(x => x.Id == tradeItem.ToUser);
						if (toUser == null)
							throw new TradeException("Reciver does not exist");

						var transfer = new TransferHistory
						{
							Amount = tradeItem.Amount,
							Fee = 0,
							CurrencyId = tradeItem.CurrencyId,
							Timestamp = DateTime.UtcNow,
							ToUserId = toUser.Id,
							TransferType = TransferType.User,
							UserId = tradeItem.UserId,
						};

						context.TransferHistory.Add(transfer);
						await context.SaveChangesAsync();


						var userNotifications = new List<NotifyUser>();
						var balanceNotifications = new List<NotifyBalanceUpdate>();
						var senderAudit = await AuditService.AuditUserCurrency(context, tradeItem.UserId, tradeItem.CurrencyId);
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
							UserId = tradeItem.UserId,
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
						Log.Warn("TradeService", "Rollback database Transaction");
						transaction.Rollback();
						Log.Warn("TradeService", ex.Message);
						return new CreateTransferResponse { Error = ex.Message };
					}
					catch (Exception ex)
					{
						Log.Error("TradeService", "Rollback databaseTransaction");
						transaction.TryRollback();
						Log.Exception("TradeService", ex);
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
						var audits = new HashSet<int>();
						var notifications = new List<INotify>();
						var user = await context.Users.FindAsync(tradeItem.UserId);
						if (user == null)
							throw new Exception("User does not exist");

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
								Avaliable = auditResult.Avaliable
							});
						}

						transaction.Commit();

						// Send notification ans invalidate cache
						await TradeNotificationService.SendNotificationCollection(notifications);
						foreach (var tradePairId in orders.Where(x => x.TradeType == TradeType.Buy).Select(x => x.TradePairId).Distinct())
						{
							CacheService.Invalidate(TradeCacheKeys.GetOpenBuyOrdersKey(tradePairId));
						}
						foreach (var tradePairId in orders.Where(x => x.TradeType == TradeType.Sell).Select(x => x.TradePairId).Distinct())
						{
							CacheService.Invalidate(TradeCacheKeys.GetOpenSellOrdersKey(tradePairId));
						}
					}
					catch (Exception ex)
					{
						Log.Error("TradeService", "Rollback databaseTransaction");
						transaction.TryRollback();
						Log.Exception("TradeService", ex);
					}
				}
			}
			return new CancelTradeResponse();
		}


		private async Task<ITradeResponse> CreateTrade(CreateTradeModel tradeRequest)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{
						Log.Info("TradeService", "Processing trade request. UserId: {0}, TradeType: {1}, TradePairId: {2}, Amount: {3:F8}, Rate: {4:F8}", tradeRequest.UserId, tradeRequest.TradeType, tradeRequest.TradePairId, tradeRequest.Amount, tradeRequest.Rate);

						var notifications = new List<INotify>();
						var response = new CreateTradeResponse();

						var tradeType = tradeRequest.TradeType;
						var user = await context.Users.FindAsync(tradeRequest.UserId);
						if (user == null)
							throw new Exception("User not found.");

						// Get or cache tradepair
						var tradePair = await context.TradePair
							.Include(t => t.Currency1)
							.Include(t => t.Currency2)
							.FirstOrDefaultAsync(x => x.Id == tradeRequest.TradePairId);
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
						IQueryable<Tables.Trade> trades;
						if (tradeType == TradeType.Buy)
						{
							// Fetch any trades that can be filled for this request
							trades = context.Trade
								.Where(o => o.TradePairId == tradeRequest.TradePairId && (o.Status == TradeStatus.Pending || o.Status == TradeStatus.Partial) && o.TradeType == TradeType.Sell && o.Rate <= tradeRate)
								.OrderBy(o => o.Rate)
								.ThenBy(o => o.Timestamp);
						}
						else
						{
							// Fetch any trades that can be filled for this request
							trades = context.Trade
								.Where(o => o.TradePairId == tradeRequest.TradePairId && (o.Status == TradeStatus.Pending || o.Status == TradeStatus.Partial) && o.TradeType == TradeType.Buy && o.Rate >= tradeRate)
								.OrderByDescending(o => o.Rate)
								.ThenBy(o => o.Timestamp);
						}

						if (trades.IsNullOrEmpty())
						{
							// There are no trades to fill, so create trade
							var trade = CreateTrade(context.Trade, tradeType, tradeRequest.UserId, tradePair, tradeAmount, tradeRate, baseCurency.TradeFee, tradeRequest.IsApi);
							Log.Debug("TradeService", "Submitting context changes...");
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
									Log.Debug("TradeService", "Filled TradeId: {0}", trade.Id);

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
									Log.Debug("TradeService", "Partially filled TradeId: {0}", trade.Id);

									//BUGFIX: if the remaining rounds out to 0.00000000 the trade is complete not partial
									if (Math.Round(trade.Remaining, 8) == 0)
									{
										trade.Remaining = 0;
										trade.Fee = 0;
										trade.Status = TradeStatus.Complete;
										Log.Debug("TradeService", "Partially filled resulted in 0.00000000 remaining, Filling TradeId: {0}", trade.Id);
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

								// Update tradepair stats
								tradePair.Change = GetChangePercent(tradePair.LastTrade, trade.Rate);
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

							// If the remaining is not 0 create an trade for the rest
							if (Math.Round(tradeRemaining, 8) > 0 && tradesLimit > 0)
							{
								// create trade for remaining
								remainingTrade = CreateTrade(context.Trade, tradeType, tradeRequest.UserId, tradePair, tradeRemaining, tradeRate, baseCurency.TradeFee, tradeRequest.IsApi);

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

							Log.Debug("TradeService", "Submitting context changes...");
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

						Log.Debug("TradeService", "Committing database transaction");
						await context.SaveChangesAsync();
						transaction.Commit();

						// Send notifications and invalidate cache
						await TradeNotificationService.SendNotificationCollection(notifications);
						CacheService.Invalidate(TradeCacheKeys.GetTradeHistoryKey(tradePair.Id));
						CacheService.Invalidate(TradeCacheKeys.GetOpenBuyOrdersKey(tradePair.Id));
						CacheService.Invalidate(TradeCacheKeys.GetOpenSellOrdersKey(tradePair.Id));

						Log.Info("TradeService", "Process trade success.");
						return response;
					}
					catch (TradeException ex)
					{
						Log.Warn("TradeService", "Rollback database Transaction");
						transaction.TryRollback();
						Log.Warn("TradeService", ex.Message);

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
						Log.Error("TradeService", "Rollback database Transaction");
						transaction.TryRollback();
						Log.Exception("TradeService", ex);

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
			Log.Debug("TradeService", "Created new trade, TradeId: {0}, TradeType: {1}, Amount: {2}, Rate: {3}", newTrade.Id, newTrade.TradeType, newTrade.Amount, newTrade.Rate);
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
			Log.Debug("TradeService", "Created new transaction, CurrencyId: {0}, TransactionType: {1}, Amount: {2}, Rate: {3}, Fee: {4}", transaction.CurrencyId, transaction.TradeHistoryType, transaction.Amount, transaction.Rate, transaction.Fee);
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

		//private async Task<bool> AuditUserTradePairAsync(IDataContext context, string userId, Entity.TradePair tradepair)
		//{
		//	var result = await AuditService.AuditUserTradePair(context, userId, tradepair);
		//	return result.Success;
		//}

		//private async Task<bool> AuditUserBalanceAsync(IDataContext context, string userId, int currencyId)
		//{
		//	var result = await AuditService.AuditUserCurrency(context, userId, currencyId);
		//	return result.Success;
		//}

		#endregion
	}

	public class TradeException : Exception
	{
		public TradeException(string message) : base(message)
		{
		}
	}

	//public class TradeNotifier
	//{
	//	public TradeNotifier(int tradePairId, INotificationService notificationService)
	//	{
	//		TradePairId = tradePairId;
	//		NotificationService = notificationService;
	//	}

	//	public int TradePairId { get; set; }
	//	public INotificationService NotificationService { get; set; }

	//	private readonly List<INotification> _notifications = new List<INotification>();
	//	private readonly List<IUserNotification> _userNotifications = new List<IUserNotification>();

	//	private readonly List<IDataNotification> _dataNotifications = new List<IDataNotification>();
	//	private readonly List<IUserDataNotification> _userDataNotifications = new List<IUserDataNotification>();

	//	private readonly List<IDataTableNotification> _dataTableNotifications = new List<IDataTableNotification>();
	//	private readonly List<IUserDataTableNotification> _userDataTableNotifications = new List<IUserDataTableNotification>();


	//	public async Task SendNotificationsAsync()
	//	{
	//		if (_dataTableNotifications.Any())
	//			await NotificationService.SendDataTableNotificationAsync(_dataTableNotifications);

	//		if (_userDataTableNotifications.Any())
	//			await NotificationService.SendUserDataTableNotificationAsync(_userDataTableNotifications);

	//		if (_dataNotifications.Any())
	//			await NotificationService.SendDataNotificationAsync(_dataNotifications);

	//		if (_userDataNotifications.Any())
	//			await NotificationService.SendUserNotificationDataAsync(_userDataNotifications);

	//		if (_notifications.Any())
	//			await NotificationService.SendNotificationAsync(_notifications);

	//		if (_userNotifications.Any())
	//			await NotificationService.SendUserNotificationAsync(_userNotifications);
	//	}

	//	internal void AddUserNotification(string userId, string message, params object[] format)
	//	{
	//		_userNotifications.Add(new UserNotification(NotificationType.Info, userId, "Trade Notification", string.Format(message, format)));
	//	}

	//	internal void AddDataTableNotification(string dataTable)
	//	{
	//		var tableName = string.Format(dataTable, TradePairId);
	//		if (!_dataTableNotifications.Any(x => x.DataTableName == tableName))
	//			_dataTableNotifications.Add(new DataTableNotification(tableName));
	//	}

	//	internal void AddDataTableNotification(string dataTable, int tradPairId)
	//	{
	//		var tableName = string.Format(dataTable, tradPairId);
	//		if (!_dataTableNotifications.Any(x => x.DataTableName == tableName))
	//			_dataTableNotifications.Add(new DataTableNotification(tableName));
	//	}

	//	internal void AddUserDataTableNotification(string userId, string dataTable)
	//	{
	//		var tableName = string.Format(dataTable, TradePairId);
	//		if (!_userDataTableNotifications.Any(x => x.UserId == userId && x.DataTableName == tableName))
	//			_userDataTableNotifications.Add(new UserDataTableNotification(userId, tableName));
	//	}

	//	internal void AddUserDataTableNotification(string userId, string dataTable, int tradePairId)
	//	{
	//		var tableName = string.Format(dataTable, tradePairId);
	//		if (!_userDataTableNotifications.Any(x => x.UserId == userId && x.DataTableName == tableName))
	//			_userDataTableNotifications.Add(new UserDataTableNotification(userId, tableName));
	//	}

	//	internal void AddDataNotification(string element, string value)
	//	{
	//		_dataNotifications.Add(new DataNotification(element, value));
	//	}

	//	internal void AddUserDataNotification(string userId, string element, string value)
	//	{
	//		_userDataNotifications.Add(new UserDataNotification(userId, element, value));
	//	}
	//}
}