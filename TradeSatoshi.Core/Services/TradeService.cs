using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Base.Queueing;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Data.Entities;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Trade;
using System.Data.Entity;
using Tables = TradeSatoshi.Common.Data.Entities;
using TradeSatoshi.Common.Services.AuditService;
using TradeSatoshi.Base.Extensions;
using TradeSatoshi.Common.Services.NotificationService;
using TradeSatoshi.Common.Logging;

namespace TradeSatoshi.Core.Services
{
	public class TradeService : ITradeService
	{
		private static ProcessorQueue<ITradeItem, ITradeResponse> _tradeProcessor = new ProcessorQueue<ITradeItem, ITradeResponse>(new TradeService().ProcessTradeItem);

		public ILogger Log { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IAuditService AuditService { get; set; }
		public INotificationService NotificationService { get; set; }

		public async Task<ITradeResponse> QueueTradeItem(ITradeItem tradeItem)
		{
			return await _tradeProcessor.QueueItem(tradeItem).ConfigureAwait(false);
		}

		private async Task<ITradeResponse> ProcessTradeItem(ITradeItem tradeRequest)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{

						Log.Info("TradeService", "Processing trade request. UserId: {0}, IsBuy: {1}, TradePairId: {2}, Amount: {3}, Rate: {4}", tradeRequest.UserId, tradeRequest.IsBuy, tradeRequest.TradePairId, tradeRequest.Amount, tradeRequest.Rate);
						var response = new TradeResponse();
						var tradeType = tradeRequest.IsBuy ? TradeHistoryType.Buy : TradeHistoryType.Sell;
						var user = context.Users.FirstOrDefault(x => x.Id == tradeRequest.UserId);
						if (user == null)
						{
							throw new Exception("User does not exist");
						}

						// Get or cache tradepair
						var tradePair = context.TradePair.Include(t => t.Currency1).Include(t => t.Currency2).FirstOrDefault(x => x.Id == tradeRequest.TradePairId);
						if (tradePair == null || tradePair.Status != TradePairStatus.OK)
						{
							throw new Exception("TradePair does not exist");
						}

						// Get or cache currency
						var currency = tradePair.Currency1;
						var baseCurency = tradePair.Currency2;

						int tradeCurrency = tradeRequest.IsBuy ? tradePair.CurrencyId2 : tradePair.CurrencyId1;
						decimal tradeRate = Math.Round(tradeRequest.Rate, 8);
						decimal tradeAmount = Math.Round(tradeRequest.Amount, 8);
						decimal tradeTotal = tradeRequest.IsBuy ? (tradeAmount * tradeRate).IncludingFees(baseCurency.TradeFee) : tradeAmount;
						decimal totalTradeAmount = tradeAmount * tradeRate;
						if (totalTradeAmount <= baseCurency.MinBaseTrade)
						{
							throw new Exception("Invalid trade amount, Minumim total trade is {0} {1}");
						}

						if (tradeAmount >= 1000000000 || totalTradeAmount >= 1000000000)
						{
							throw new Exception("Invalid trade amount, Maximum single trade is 1000000000 coins");
						}

						if (tradeRate < 0.00000001m)
						{
							throw new Exception("Invalid trade price, minimum price is 0.00000001 {0}");
						}

						if (tradeRate > 1000000000)
						{
							throw new Exception("Invalid trade price, maximum price is 1000000000 {0}");
						}

						//Audit the user balance for currency
						if (!AuditUserBalance(context, tradeRequest.UserId, tradeCurrency))
						{
							throw new Exception(string.Format("Failed to audit user balance, Currency: {0}", tradeCurrency));
						}

						var tradeRequestBalance = context.Balance.FirstOrDefault(b => b.UserId == tradeRequest.UserId && b.CurrencyId == tradeCurrency);
						if (tradeRequestBalance == null || tradeRequestBalance.Avaliable < tradeTotal)
						{
							throw new Exception("Insufficient Funds");
						}

						// Get existing trades that can be filled
						IQueryable<Tables.Trade> trades = null;
						if (tradeType == TradeHistoryType.Buy)
						{
							// Fetch any trades that can be filled for this request
							trades = context.Trade
								.Where(o => o.TradePairId == tradeRequest.TradePairId && (o.Status == TradeStatus.Pending || o.Status == TradeStatus.Partial) && o.TradeType == TradeHistoryType.Sell && o.Rate <= tradeRate)
								.OrderBy(o => o.Rate)
								.ThenBy(o => o.Timestamp)
								.AsQueryable();
						}
						else
						{
							// Fetch any trades that can be filled for this request
							trades = context.Trade
								.Where(o => o.TradePairId == tradeRequest.TradePairId && (o.Status == TradeStatus.Pending || o.Status == TradeStatus.Partial) && o.TradeType == TradeHistoryType.Buy && o.Rate >= tradeRate)
								.OrderByDescending(o => o.Rate)
								.ThenBy(o => o.Timestamp)
								.AsQueryable();
						}

						if (trades.IsNullOrEmpty())
						{
							// There are no trades to fill, so create trade
							var trade = CreateTrade(context.Trade, tradeType, tradeRequest.UserId, tradePair, tradeAmount, tradeRate, baseCurency.TradeFee, tradeRequest.IsApi);

							response.AddNotification(new TradeNotification(NotificationType.Info, tradeRequest.UserId, string.Format("Trade placed.{0}{1} {2:F8} {3} @ {4:F8} {5}", Environment.NewLine, tradeType, tradeAmount, currency.Symbol, tradeRate, baseCurency.Symbol)));
							response.AddNotification(new TradeDataNotification(TradeNotificationType.TradePairUserData, tradeRequest.TradePairId, tradeRequest.UserId));
							response.AddNotification(new TradeDataNotification(TradeNotificationType.TradePairData, tradeRequest.TradePairId));

							Log.Debug("TradeService", "Submitting context changes...");
							context.SaveChanges();

							if (!AuditUserTradePair(context, tradeRequest.UserId, tradePair))
							{
								throw new Exception("Failed to audit user balances.");
							}

							response.TradeId = trade.Id;
						}
						else
						{
							var audits = new HashSet<string> { user.Id };
							int tradesLimit = 200;
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
								if (tradeType == TradeHistoryType.Buy)
								{
									// Create transaction for the buy
									newTransactions.Add(CreateTransaction(context.TradeHistory, TradeHistoryType.Buy, tradeRequest.UserId, trade.UserId, tradePair, dogeAmount, trade.Rate, trade.Fee > 0 ? baseCurency.TradeFee : 0, tradeRequest.IsApi));

									// If the rate is cheaper than the buyers request, sort out a refund amount for notification
									buyersRefund += (expectedBtcAmount - actualBtcAmount);
								}
								else
								{
									// Create transaction for this fill
									newTransactions.Add(CreateTransaction(context.TradeHistory, TradeHistoryType.Sell, trade.UserId, tradeRequest.UserId, tradePair, dogeAmount, trade.Rate, trade.Fee > 0 ? baseCurency.TradeFee : 0, tradeRequest.IsApi));
								}

								if (canFillTrade)
								{
									// We can fill this whole trade so zero out and mark as 'Complete'
									trade.Remaining = 0;
									trade.Fee = 0;
									trade.Status = TradeStatus.Complete;
									tradeRemaining -= dogeAmount;
									Log.Debug("TradeService", "Filled TradeId: {0}", trade.Id);
								}
								else
								{
									// We can only fill some of this trade, subtract the amount and mark at 'Partial'
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
								}

								// Update tradepair stats
								tradePair.Change = GetChangePercent(tradePair.LastTrade, trade.Rate);
								tradePair.LastTrade = trade.Rate;

								// Add to Audit list
								audits.Add(trade.UserId);

								// Add Notifications
								response.AddNotification(new TradeNotification(NotificationType.Info, tradeRequest.UserId, string.Format("You {0} {1:F8} {2} for {3:F8} {4}", tradeType == TradeHistoryType.Buy ? "bought" : "sold", dogeAmount, currency.Symbol, actualBtcAmount, baseCurency.Symbol)));
								response.AddNotification(new TradeNotification(NotificationType.Info, trade.UserId, string.Format("You {0} {1:F8} {2} for {3:F8} {4}", tradeType == TradeHistoryType.Buy ? "Sold" : "bought", dogeAmount, currency.Symbol, actualBtcAmount, baseCurency.Symbol)));

								// Update trade limiter
								tradesLimit--;
							}

							// If the remaining is not 0 create an trade for the rest
							if (Math.Round(tradeRemaining, 8) > 0 && tradesLimit > 0)
							{
								// create trade for remaining
								remainingTrade = CreateTrade(context.Trade, tradeType, tradeRequest.UserId, tradePair, tradeRemaining, tradeRate, baseCurency.TradeFee, tradeRequest.IsApi);
							}

							if (tradeType == TradeHistoryType.Buy && buyersRefund > 0)
							{
								response.Notifications.Add(new TradeNotification(NotificationType.Info, tradeRequest.UserId, string.Format("Refunded {0:F8} {1} form buy trade", buyersRefund, baseCurency.Symbol)));
							}

							response.AddNotification(new TradePriceNotification(tradeRequest.TradePairId, tradePair.LastTrade, tradePair.Change));
						    response.AddNotification(new TradeDataNotification(TradeNotificationType.TradePairData, tradeRequest.TradePairId));

							Log.Debug("TradeService", "Submitting context changes...");
							context.SaveChanges();

							// Audit all users involved
							foreach (var userAudit in audits)
							{
								if (!AuditUserTradePair(context, userAudit, tradePair))
								{
									throw new Exception("Failed to audit user balances.");
								}
								response.AddNotification(new TradeDataNotification(TradeNotificationType.TradePairUserData, tradeRequest.TradePairId, userAudit));
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
							}
						}

						Log.Debug("TradeService", "Committing database transaction");
						context.SaveChanges();
						transaction.Commit();

						Log.Info("TradeService", "Process trade success.");
						return response;
					}
					catch (Exception ex)
					{
						Log.Error("TradeService", "Rollback databaseTransaction");
						transaction.Rollback();
						Log.Exception("TradeService", ex);
					}
				}
			}

			return null;
		}











		private Tables.Trade CreateTrade(DbSet<Tables.Trade> tradeTable, TradeHistoryType type, string userId, TradePair tradePair, decimal amount, decimal rate, decimal fee, bool isapi)
		{
			var newTrade = new Tables.Trade();
			newTrade.TradePairId = tradePair.Id;
			newTrade.Amount = Math.Round(amount, 8);
			newTrade.Rate = Math.Round(rate, 8);
			newTrade.Remaining = Math.Round(amount, 8);
			newTrade.Status = TradeStatus.Pending;
			newTrade.TradeType = type;
			newTrade.UserId = userId;
			newTrade.Timestamp = DateTime.UtcNow;
			newTrade.Fee = (amount * rate).GetFees(fee);
			newTrade.IsApi = isapi;
			tradeTable.Add(newTrade);
			Log.Debug("TradeService", "Created new trade, TradeId: {0}, TradeType: {1}, Amount: {2}, Rate: {3}"
										, newTrade.Id, newTrade.TradeType, newTrade.Amount, newTrade.Rate);
			return newTrade;
		}


		private Tables.TradeHistory CreateTransaction(DbSet<Tables.TradeHistory> transactionTable, TradeHistoryType type, string userId, string toUserId, TradePair tradepair, decimal amount, decimal rate, decimal fee, bool isapi)
		{
			var transaction = new TradeHistory();
			transaction.TradePairId = tradepair.Id;
			transaction.CurrencyId = tradepair.CurrencyId1;
			transaction.Amount = Math.Round(amount, 8);
			transaction.Rate = Math.Round(rate, 8);
			transaction.TradeHistoryType = type;
			transaction.UserId = userId;
			transaction.ToUserId = toUserId;
			transaction.Timestamp = DateTime.UtcNow;
			transaction.Fee = (amount * rate).GetFees(fee);
			transaction.IsApi = isapi;
			transactionTable.Add(transaction);
			Log.Debug("TradeService", "Created new transaction, CurrencyId: {0}, TransactionType: {1}, Amount: {2}, Rate: {3}, Fee: {4}"
								  , transaction.CurrencyId, transaction.TradeHistoryType, transaction.Amount, transaction.Rate, transaction.Fee);
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

		private bool AuditUserTradePair(IDataContext context, string userId, TradePair tradepair)
		{
			var result = AuditService.AuditUserTradePair(context, userId, tradepair);
			return result.Success;
		}

		private bool AuditUserBalance(IDataContext context, string userId, int currencyId)
		{
			var result = AuditService.AuditUserCurrency(context, userId, currencyId);
			return result.Success;
		}
	}

	public interface ITradeNotification
	{

	}

	public class TradeNotification : ITradeNotification
	{
		public NotificationType NotificationType { get; set; }
		public string UserId { get; set; }
		public string Message { get; set; }

		public TradeNotification(NotificationType notificationType, string userId, string message)
		{
			NotificationType = notificationType;
			UserId = userId;
			Message = message;
		}

	}

	public class TradeResponse : ITradeResponse
	{
		public TradeResponse()
		{
			FilledTrades = new List<int>();
			Notifications = new List<ITradeNotification>();
		}

		public string Error { get; set; }
		public string Message { get; set; }
		public int? TradeId { get; set; }
		public List<int> FilledTrades { get; set; }
		public List<ITradeNotification> Notifications { get; set; }

		public void AddNotification(ITradeNotification notification)
		{
			Notifications.Add(notification);
		}

		public void AddFilledTrade(int tradeId)
		{
			FilledTrades.Add(tradeId);
		}
	}

	public enum TradeNotificationType
	{
		TradePairData,
		TradePairUserData
	}

	public class TradeDataNotification : ITradeNotification
	{
		public TradeDataNotification(TradeNotificationType type, int tradepair, string userId = null)
		{
			Type = type;
			UserId = userId;
			TradePair = tradepair;
		}

		public TradeNotificationType Type { get; set; }
		public int TradePair { get; set; }
		public string UserId { get; set; }
	}

	public class TradePriceNotification : ITradeNotification
	{
		public TradePriceNotification(int tradepair, decimal price, double change)
		{
			TradePair = tradepair;
			Price = price;
			Change = change;
		}


		public int TradePair { get; set; }
		public decimal Price { get; set; }
		public double Change { get; set; }
	}
}
