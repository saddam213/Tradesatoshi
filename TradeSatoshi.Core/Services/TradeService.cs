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
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Core.Logger;

namespace TradeSatoshi.Core.Services
{
	public class TradeService : ITradeService
	{
		private static ProcessorQueue<ITradeItem, ITradeResponse> _tradeProcessor =
		 new ProcessorQueue<ITradeItem, ITradeResponse>(new TradeService(new DatabaseLogger(new DataContextFactory()), new DataContextFactory(), new AuditService(), new NotificationService()).ProcessTradeItem);

		public ILogger Log { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IAuditService AuditService { get; set; }
		public INotificationService NotificationService { get; set; }

		public TradeService() { }
		public TradeService(ILogger log, IDataContextFactory contextFactory, IAuditService auditService, INotificationService notificationService)
		{
			Log = log;
			AuditService = auditService;
			DataContextFactory = contextFactory;
			NotificationService = notificationService;
		}

		public async Task<ITradeResponse> QueueTradeItem(ITradeItem tradeItem)
		{
			return await _tradeProcessor.QueueItem(tradeItem).ConfigureAwait(false);
		}

		private async Task<ITradeResponse> ProcessTradeItem(ITradeItem tradeItem)
		{
			if (tradeItem is CreateTradeModel)
			{
				return await CreateTrade(tradeItem as CreateTradeModel);
			}
			else if (tradeItem is CancelTradeModel)
			{
				return await CancelTrade(tradeItem as CancelTradeModel);
			}
			return null;
		}

		private async Task<ITradeResponse> CancelTrade(CancelTradeModel tradeItem)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{
						var user = await context.Users.FindAsync(tradeItem.UserId);
						if (user == null)
						{
							throw new Exception("User does not exist");
						}

						var audits = new HashSet<int>();
						var response = new CancelTradeResponse();
						var notifications = new TradeNotifier(tradeItem.TradePairId, NotificationService);
						if (tradeItem.CancelType == CancelTradeType.Trade)
						{
							Log.Info("TradeService", "Processing cancel trade request. UserId: {0}, CancelType: Trade, TradeId: {1}", tradeItem.UserId, tradeItem.TradeId);
							var trade = await context.Trade.FirstOrDefaultAsync(x => x.Id == tradeItem.TradeId && x.UserId == tradeItem.UserId);
							if (trade == null)
							{
								throw new Exception(string.Format("Trade #{0} does not exist", tradeItem.TradeId));
							}

							var tradePair = await context.TradePair
								.Include(t => t.Currency1)
								.Include(t => t.Currency2)
								.FirstOrDefaultAsync(x => x.Id == trade.TradePairId);
							if (tradePair == null)
							{
								throw new Exception("TradePair does not exist");
							}

							Log.Info("TradeService", "Canceling trade. UserId: {0}, TradeId: {1}", tradeItem.UserId, trade.Id);
							trade.Status = TradeStatus.Canceled;
							audits.Add(trade.TradeType == TradeType.Buy ? tradePair.CurrencyId2 : tradePair.CurrencyId1);

							response.AddCanceledOrder(trade.Id);

							notifications.AddUserNotification(trade.UserId, "Canceled {0} Order - {2:F8}{3} @ {4:F8}{5}", trade.TradeType, trade.Id, trade.Amount, tradePair.Currency1.Symbol, trade.Rate, tradePair.Currency2.Symbol);
							notifications.AddDataTableNotification(trade.TradeType == TradeType.Buy ? NotificationConstants.DataTable_BuyOrders : NotificationConstants.DataTable_SellOrders, tradePair.Id);
							notifications.AddUserDataTableNotification(tradeItem.UserId, NotificationConstants.DataTable_UserOpenOrders, tradePair.Id);

							//Cache.InvalidateTradeData(tradePair.Id);
						}
						else if (tradeItem.CancelType == CancelTradeType.TradePair)
						{
							Log.Info("TradeService", "Processing cancel trade request. UserId: {0}, CancelType: TradePair, TradePairId: {1}", tradeItem.UserId, tradeItem.TradePairId);

							var tradePair = await context.TradePair
								.Include(t => t.Currency1)
								.Include(t => t.Currency2)
								.FirstOrDefaultAsync(x => x.Id == tradeItem.TradePairId);
							if (tradePair == null)
							{
								throw new Exception("TradePair does not exist");
							}

							var trades = await context.Trade
								.Where(o => o.UserId == tradeItem.UserId && o.TradePairId == tradeItem.TradePairId && (o.Status == TradeStatus.Partial || o.Status == TradeStatus.Pending))
								.ToListAsync();
							if (trades.IsNullOrEmpty())
							{
								throw new Exception("No trades found to cancel for tradepair.");
							}

							foreach (var trade in trades)
							{
								Log.Info("TradeService", "Canceling trade. UserId: {0}, TradeId: {1}", tradeItem.UserId, trade.Id);
								trade.Status = TradeStatus.Canceled;
								response.AddCanceledOrder(trade.Id);
								notifications.AddUserNotification(trade.UserId, "Canceled {0} Trade #{1}", trade.TradeType, trade.Id);
							}
							audits.Add(tradePair.CurrencyId1);
							audits.Add(tradePair.CurrencyId2);

							if (trades.Any(x => x.TradeType == TradeType.Buy))
								notifications.AddDataTableNotification(NotificationConstants.DataTable_BuyOrders, tradePair.Id);

							if (trades.Any(x => x.TradeType == TradeType.Sell))
								notifications.AddDataTableNotification(NotificationConstants.DataTable_SellOrders, tradePair.Id);

							notifications.AddUserDataTableNotification(tradeItem.UserId, NotificationConstants.DataTable_UserOpenOrders, tradePair.Id);
							//Cache.InvalidateTradeData(tradePair.Id);
						}
						else if (tradeItem.CancelType == CancelTradeType.All)
						{
							Log.Info("TradeService", "Processing cancel trade request. UserId: {0}, CancelType: All", tradeItem.UserId);

							var trades = await context.Trade
								.Include(t => t.TradePair)
								.Where(o => o.UserId == tradeItem.UserId && (o.Status == TradeStatus.Partial || o.Status == TradeStatus.Pending))
								.ToListAsync();
							if (trades.IsNullOrEmpty())
							{
								throw new Exception("No trades found to cancel for tradepair.");
							}

							Log.Info("TradeService", "Canceling all trades... TradeCount: {0}", trades.Count());

							foreach (var group in trades.GroupBy(o => o.TradePairId))
							{
								foreach (var trade in group)
								{
									Log.Info("TradeService", "Canceling trade. UserId: {0}, TradeId: {1}", tradeItem.UserId, trade.Id);
									trade.Status = TradeStatus.Canceled;
									response.AddCanceledOrder(trade.Id);
									notifications.AddUserNotification(trade.UserId, "Canceled {0} Trade #{1}", trade.TradeType, trade.Id);
									audits.Add(trade.TradePair.CurrencyId1);
									audits.Add(trade.TradePair.CurrencyId2);
								}

								if (group.Any(x => x.TradeType == TradeType.Buy))
									notifications.AddDataTableNotification(NotificationConstants.DataTable_BuyOrders, group.Key);

								if (group.Any(x => x.TradeType == TradeType.Sell))
									notifications.AddDataTableNotification(NotificationConstants.DataTable_SellOrders, group.Key);

								notifications.AddUserDataTableNotification(tradeItem.UserId, NotificationConstants.DataTable_UserOpenOrders, group.Key);
								//Cache.InvalidateTradeData(tradePair.Id);
							}
						}

						// Submit changes to context
						Log.Debug("TradeService", "Submitting context changes...");
						await context.SaveChangesAsync();


						//Audit the currency associated with the canceled trade
						foreach (var auditCurrency in audits)
						{
							if (!await AuditUserBalanceAsync(context, tradeItem.UserId, auditCurrency, notifications))
							{
								throw new Exception("Failed to audit user balance.");
							}
						}

						transaction.Commit();

						await notifications.SendNotificationsAsync();
					
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

		private async Task<ITradeResponse> CreateTrade(CreateTradeModel tradeRequest)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				using (var transaction = context.Database.BeginTransaction())
				{
					try
					{

						Log.Info("TradeService", "Processing trade request. UserId: {0}, TradeType: {1}, TradePairId: {2}, Amount: {3:F8}, Rate: {4:F8}", tradeRequest.UserId, tradeRequest.TradeType, tradeRequest.TradePairId, tradeRequest.Amount, tradeRequest.Rate);
						var response = new CreateTradeResponse();
						var notifications = new TradeNotifier(tradeRequest.TradePairId, NotificationService);

						var tradeType = tradeRequest.TradeType;
						var user = await context.Users.FindAsync(tradeRequest.UserId);
						if (user == null)
						{
							throw new Exception("User not found.");
						}

						// Get or cache tradepair
						var tradePair = await context.TradePair
							.Include(t => t.Currency1)
							.Include(t => t.Currency2)
							.FirstOrDefaultAsync(x => x.Id == tradeRequest.TradePairId);
						if (tradePair == null)
						{
							throw new Exception("Market not found.");
						}

						if (tradePair.Status != TradePairStatus.OK)
						{
							throw new Exception(string.Format("Market status is currently {0}, unable to process trade.", tradePair.Status));
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
						{
							throw new Exception(string.Format("Invalid trade amount, Minumim total trade is {0} {1}.", baseCurency.MinBaseTrade, baseCurency.Symbol));
						}

						if (tradeAmount >= 1000000000 || totalTradeAmount >= 1000000000)
						{
							throw new Exception("Invalid trade amount, Maximum single trade is 1000000000 coins");
						}

						if (tradeRate < 0.00000001m)
						{
							throw new Exception(string.Format("Invalid trade price, minimum price is 0.00000001 {0}", baseCurency.Symbol));
						}

						if (tradeRate > 1000000000)
						{
							throw new Exception(string.Format("Invalid trade price, maximum price is 1000000000 {0}", baseCurency.Symbol));
						}

						//Audit the user balance for currency
						if (!await AuditUserBalanceAsync(context, tradeRequest.UserId, tradeCurrency))
						{
							throw new Exception(string.Format("Failed to audit user balance, Currency: {0}", tradeCurrency));
						}

						var tradeRequestBalance = await context.Balance.FirstOrDefaultAsync(b => b.UserId == tradeRequest.UserId && b.CurrencyId == tradeCurrency);
						if (tradeRequestBalance == null || tradeRequestBalance.Avaliable < tradeTotal)
						{
							throw new Exception("Insufficient funds for trade.");
						}

						// Get existing trades that can be filled
						IQueryable<Tables.Trade> trades = null;
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

							notifications.AddUserNotification(tradeRequest.UserId, "Trade placed.{0}{1} {2:F8} {3} @ {4:F8} {5}", Environment.NewLine, tradeType, tradeAmount, currency.Symbol, tradeRate, baseCurency.Symbol);
							notifications.AddDataTableNotification(tradeType == TradeType.Sell ? NotificationConstants.DataTable_SellOrders : NotificationConstants.DataTable_BuyOrders);
							notifications.AddDataTableNotification(NotificationConstants.DataTable_UserOpenOrders);

							Log.Debug("TradeService", "Submitting context changes...");
							await context.SaveChangesAsync();

							if (!await AuditUserTradePairAsync(context, tradeRequest.UserId, tradePair, notifications))
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
								notifications.AddUserNotification(tradeRequest.UserId, "You {0} {1:F8} {2} for {3:F8} {4}", tradeType == TradeType.Buy ? "bought" : "sold", dogeAmount, currency.Symbol, actualBtcAmount, baseCurency.Symbol);
								notifications.AddUserNotification(trade.UserId, "You {0} {1:F8} {2} for {3:F8} {4}", tradeType == TradeType.Buy ? "Sold" : "bought", dogeAmount, currency.Symbol, actualBtcAmount, baseCurency.Symbol);

								// Update trade limiter
								tradesLimit--;
							}

							// If the remaining is not 0 create an trade for the rest
							if (Math.Round(tradeRemaining, 8) > 0 && tradesLimit > 0)
							{
								// create trade for remaining
								remainingTrade = CreateTrade(context.Trade, tradeType, tradeRequest.UserId, tradePair, tradeRemaining, tradeRate, baseCurency.TradeFee, tradeRequest.IsApi);
							}

							if (tradeType == TradeType.Buy && buyersRefund > 0)
							{
								notifications.AddUserNotification(tradeRequest.UserId, "Refunded {0:F8} {1} form buy trade", buyersRefund, baseCurency.Symbol);
							}

							notifications.AddDataNotification(string.Format(NotificationConstants.Data_LastPrice, tradePair.Id), tradePair.LastTrade.ToString("F8"));
							notifications.AddDataTableNotification(NotificationConstants.DataTable_BuyOrders);
							notifications.AddDataTableNotification(NotificationConstants.DataTable_SellOrders);
							notifications.AddDataTableNotification(NotificationConstants.DataTable_TradeHistory);

							Log.Debug("TradeService", "Submitting context changes...");
							await context.SaveChangesAsync();

							// Audit all users involved
							foreach (var userAudit in audits)
							{
								if (!await AuditUserTradePairAsync(context, userAudit, tradePair, notifications))
								{
									throw new Exception("Failed to audit user balances.");
								}
								notifications.AddUserDataTableNotification(userAudit, NotificationConstants.DataTable_UserOpenOrders);
								notifications.AddUserDataTableNotification(userAudit, NotificationConstants.DataTable_UserTradeHistory);
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
						await context.SaveChangesAsync();
						transaction.Commit();

						await notifications.SendNotificationsAsync();

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

		#region Helpers

		private Tables.Trade CreateTrade(DbSet<Tables.Trade> tradeTable, TradeType type, string userId, TradePair tradePair, decimal amount, decimal rate, decimal fee, bool isapi)
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

		private Tables.TradeHistory CreateTransaction(DbSet<Tables.TradeHistory> transactionTable, TradeType type, string userId, string toUserId, TradePair tradepair, decimal amount, decimal rate, decimal fee, bool isapi)
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

		private async Task<bool> AuditUserTradePairAsync(IDataContext context, string userId, TradePair tradepair, TradeNotifier notifications)
		{
			var result = await AuditService.AuditUserTradePairAsync(context, userId, tradepair);
			if (result.Success)
			{
				notifications.AddUserDataNotification(userId, string.Format(NotificationConstants.Data_UserBalance, result.Symbol), string.Format("{0:F8} {1}", result.Available, result.Symbol));
				notifications.AddUserDataNotification(userId, string.Format(NotificationConstants.Data_UserBalance, result.BaseSymbol), string.Format("{0:F8} {1}", result.BaseAvailable, result.BaseSymbol));
			}
			return result.Success;
		}

		private async Task<bool> AuditUserBalanceAsync(IDataContext context, string userId, int currencyId, TradeNotifier notifications)
		{
			var result = await AuditService.AuditUserCurrencyAsync(context, userId, currencyId);
			if (result.Success)
			{
				notifications.AddUserDataNotification(userId, string.Format(NotificationConstants.Data_UserBalance, result.Symbol), string.Format("{0:F8} {1}", result.Available, result.Symbol));
			}
			return result.Success;
		}

		private async Task<bool> AuditUserBalanceAsync(IDataContext context, string userId, int currencyId)
		{
			var result = await AuditService.AuditUserCurrencyAsync(context, userId, currencyId);
			return result.Success;
		}

		#endregion
	}

	public class TradeNotifier
	{
		public TradeNotifier(int tradePairId, INotificationService notificationService)
		{
			TradePairId = tradePairId;
			NotificationService = notificationService;
		}

		public int TradePairId { get; set; }
		public INotificationService NotificationService { get; set; }

		private List<INotification> _notifications = new List<INotification>();
		private List<IUserNotification> _userNotifications = new List<IUserNotification>();

		private List<IDataNotification> _dataNotifications = new List<IDataNotification>();
		private List<IUserDataNotification> _userDataNotifications = new List<IUserDataNotification>();

		private List<IDataTableNotification> _dataTableNotifications = new List<IDataTableNotification>();
		private List<IUserDataTableNotification> _userDataTableNotifications = new List<IUserDataTableNotification>();


		public async Task SendNotificationsAsync()
		{
			if (_dataTableNotifications.Any())
				await NotificationService.SendDataTableNotificationAsync(_dataTableNotifications);

			if (_userDataTableNotifications.Any())
				await NotificationService.SendUserDataTableNotificationAsync(_userDataTableNotifications);

			if (_dataNotifications.Any())
				await NotificationService.SendDataNotificationAsync(_dataNotifications);

			if (_userDataNotifications.Any())
				await NotificationService.SendUserNotificationDataAsync(_userDataNotifications);

			if (_notifications.Any())
				await NotificationService.SendNotificationAsync(_notifications);

			if (_userNotifications.Any())
				await NotificationService.SendUserNotificationAsync(_userNotifications);
		}

		internal void AddUserNotification(string userId, string message, params object[] format)
		{
			_userNotifications.Add(new UserNotification(NotificationType.Info, userId, "Trade Notification", string.Format(message, format)));
		}

		internal void AddDataTableNotification(string dataTable)
		{
			var tableName = string.Format(dataTable, TradePairId);
			if (!_dataTableNotifications.Any(x => x.DataTableName == tableName))
				_dataTableNotifications.Add(new DataTableNotification(tableName));
		}

		internal void AddDataTableNotification(string dataTable, int tradPairId)
		{
			var tableName = string.Format(dataTable, tradPairId);
			if (!_dataTableNotifications.Any(x => x.DataTableName == tableName))
				_dataTableNotifications.Add(new DataTableNotification(tableName));
		}

		internal void AddUserDataTableNotification(string userId, string dataTable)
		{
			var tableName = string.Format(dataTable, TradePairId);
			if (!_userDataTableNotifications.Any(x => x.UserId == userId && x.DataTableName == tableName))
				_userDataTableNotifications.Add(new UserDataTableNotification(userId, tableName));
		}

		internal void AddUserDataTableNotification(string userId, string dataTable, int tradePairId)
		{
			var tableName = string.Format(dataTable, tradePairId);
			if (!_userDataTableNotifications.Any(x => x.UserId == userId && x.DataTableName == tableName))
				_userDataTableNotifications.Add(new UserDataTableNotification(userId, tableName));
		}

		internal void AddDataNotification(string element, string value)
		{
			_dataNotifications.Add(new DataNotification(element, value));
		}

		internal void AddUserDataNotification(string userId, string element, string value)
		{
			_userDataNotifications.Add(new UserDataNotification(userId, element, value));
		}
	}
}
