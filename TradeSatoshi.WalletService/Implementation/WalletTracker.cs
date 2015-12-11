using Cryptopia.WalletAPI.Base;
using Cryptopia.WalletAPI.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Base.Extensions;
using System.Data.Entity;
using TradeSatoshi.Common.Services.NotificationService;
using TradeSatoshi.Core.Services;
using TradeSatoshi.Base.Logging;
using TradeSatoshi.Common.Data.Entities;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.AuditService;

namespace TradeSatoshi.WalletService.Implementation
{
	public class WalletTracker
	{
		private readonly TradeSatoshi.Base.Logging.Log Log = LoggingManager.GetLog(typeof(WalletTracker));
		private CancellationToken _cancelToken;
		private bool _isRunning = false;
		private bool _isEnabled = false;
		private int _pollPeriod = 30;
		public IAuditService AuditService { get; set; }
		public INotificationService NotificationService { get; set; }

		public WalletTracker(CancellationToken cancelToken)
		{
			_isRunning = true;
			_isEnabled = true;
			_cancelToken = cancelToken;
			AuditService = new AuditService();
			NotificationService = new NotificationService();
			Task.Factory.StartNew(async () => await Process(), _cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
		}

		public bool Running
		{
			get { return _isRunning; }
		}

		private async Task Process()
		{
			while (_isEnabled)
			{
				try
				{
					Log.Message(LogLevel.Info, "Processing wallets...");
					await ProcessInfo();
					await ProcessDeposits();
					Log.Message(LogLevel.Info, "Processing wallets complete.");
					await Task.Delay(TimeSpan.FromSeconds(_pollPeriod), _cancelToken);
				}
				catch (TaskCanceledException)
				{
					break;
				}
			}
			_isRunning = false;
		}

		public void Stop()
		{
			_isEnabled = false;
		}

		private async Task ProcessInfo()
		{
			try
			{
				Log.Message(LogLevel.Info, "Processing wallet information...");
				using (var context = new DataContext())
				{
					var currencies = await context.Currency.Where(x => x.IsEnabled).ToListAsync();
					foreach (var currency in currencies)
					{
						try
						{
							Log.Message(LogLevel.Info, "Processing {0} wallet information...", currency.Symbol);
							if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline)
							{
								Log.Message(LogLevel.Info, "Skipping {0} wallet, Status: {1}", currency.Symbol, currency.Status);
								continue;
							}

							var currencyInfo = await GetInfo(currency).ConfigureAwait(false);
							if (currencyInfo == null)
							{
								Log.Message(LogLevel.Info, "No response from {0} wallet.", currency.Symbol);
								continue;
							}

							currency.Balance = currencyInfo.Balance;
							currency.Block = currencyInfo.Blocks;
							currency.Connections = currencyInfo.Connections;
							currency.Errors = currencyInfo.Errors;
							currency.Version = currencyInfo.Version;
							Log.Message(LogLevel.Info, "Processed {0} wallet information.", currency.Symbol);
						}
						catch (Exception ex)
						{
							Log.Exception("An exception occured processing wallet information. Currency: {0}", ex, currency.Symbol);
						}
					}
					await context.SaveChangesAsync();
				}
				Log.Message(LogLevel.Info, "Processed wallet information.");
			}
			catch (Exception ex)
			{
				Log.Exception("An exception occured processing wallet information.", ex);
			}
		}

		private async Task ProcessDeposits()
		{
			Log.Message(LogLevel.Info, "Processing deposits...");
			using (var context = new DataContext())
			{
				var userIds = await context.Users.Select(x => x.Id).ToListAsync();
				var currencies = await context.Currency.Where(x => x.IsEnabled).ToListAsync();
				foreach (var currency in currencies)
				{
					try
					{
						Log.Message(LogLevel.Info, "Processing {0} deposits...", currency.Symbol);
						if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline)
							continue; // currency is skipped

						var walletTransactions = await GetTransactions(currency, TransactionDataType.Deposit).ConfigureAwait(false);
						if (walletTransactions.IsNullOrEmpty())
						{
							Log.Message(LogLevel.Info, "No {0} deposits.", currency.Symbol);
							continue; // no deposits
						}

						var existingDeposits = await context.Deposit.Where(x => x.CurrencyId == currency.Id).ToListAsync();
						foreach (var walletDeposit in walletTransactions.OrderBy(x => x.Time))
						{
							try
							{
								var userId = userIds.FirstOrDefault(id => id == walletDeposit.Account);
								if (string.IsNullOrEmpty(userId))
									continue; // user not found

								var existingDeposit = existingDeposits.FirstOrDefault(x => x.Txid == walletDeposit.Txid && x.UserId == userId);
								if (existingDeposit == null)
								{
									// deposit does not exist;
									Log.Message(LogLevel.Info, "Processing new {0} deposit. User: {1}, Amount: {2:F8}, TxId: {3}", currency.Symbol, userId, walletDeposit.Amount, walletDeposit.Txid);
									var newDeposit = new Deposit
									{
										Amount = walletDeposit.Amount,
										Confirmations = walletDeposit.Confirmations,
										DepositType = DepositType.Normal,
										TimeStamp = walletDeposit.Time.ToDateTime(),
										Txid = walletDeposit.Txid,
										UserId = userId,
										CurrencyId = currency.Id,
										DepositStatus = walletDeposit.Confirmations >= currency.MinConfirmations
											? DepositStatus.Confirmed
											: DepositStatus.UnConfirmed
									};
									context.Deposit.Add(newDeposit);
									await context.SaveChangesAsync();
									await AuditUser(context, currency.Id, userId);
									await NotificationService.SendUserNotificationAsync(userId, new Notification
									{
										Title = "New Deposit",
										Message = string.Format("{0} Deposit #{1}, {2:F8} {3}", newDeposit.DepositStatus, newDeposit.Id, newDeposit.Amount, currency.Symbol),
										Type = NotificationType.Info
									});
									Log.Message(LogLevel.Info, "Successfully Processed new {0} deposit.", currency.Symbol);
									continue;
								}
								else
								{
									// deposit  exists;
									if (existingDeposit.DepositStatus == DepositStatus.Confirmed)
										continue; // its already confirmed

									if (existingDeposit.Confirmations == walletDeposit.Confirmations)
										continue; // no new confirms

									Log.Message(LogLevel.Info, "Processing existing {0} deposit. DepositId: {1}, Confirms: {2}", currency.Symbol, existingDeposit.Id, walletDeposit.Confirmations);
									existingDeposit.Confirmations = walletDeposit.Confirmations;
									if (existingDeposit.Confirmations >= currency.MinConfirmations)
									{

										existingDeposit.DepositStatus = DepositStatus.Confirmed;
									}

									await context.SaveChangesAsync();

									if (existingDeposit.DepositStatus == DepositStatus.Confirmed)
									{
										Log.Message(LogLevel.Info, "Deposit #{0} confirmed.", existingDeposit.Id);
										await AuditUser(context, currency.Id, userId);
										await NotificationService.SendUserNotificationAsync(userId, new Notification
										{
											Title = "Deposit Confirmed",
											Message = string.Format("Deposit #{0}, {1:F8} {2} has now been confirmed", existingDeposit.Id, existingDeposit.Amount, existingDeposit.Currency.Symbol),
											Type = NotificationType.Info
										});
									}
									Log.Message(LogLevel.Info, "Successfully Processed existing {0} deposit.", currency.Symbol);
								}
							}
							catch (Exception ex)
							{
								Log.Exception("An exception occured processing transaction. Currency: {0}, TxId: {1}", ex, currency.Symbol, walletDeposit.Txid);
							}
						}
					}
					catch (Exception ex)
					{
						Log.Exception("An exception occured processing wallet transactions. Currency: {0}", ex, currency.Symbol);
					}
				}
			}
		}

		private async Task ProcessWithdrawals()
		{
			Log.Message(LogLevel.Info, "Processing withdrawals...");
			using (var context = new DataContext())
			{
				var currencies = await context.Currency.Where(x => x.IsEnabled).ToListAsync();
				foreach (var currency in currencies)
				{
					await SendWithdrawals(context, currency);
					await UpdateWithdrawConfirmations(context, currency);
				}
			}
			Log.Message(LogLevel.Info, "Processing withdrawals complete.");
		}

		private async Task UpdateWithdrawConfirmations(IDataContext context, Currency currency)
		{
			try
			{
				Log.Message(LogLevel.Info, "Processing {0} withdrawals...", currency.Symbol);
				if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline)
					return; // currency is skipped

				var walletTransactions = await GetTransactions(currency, TransactionDataType.Withdraw).ConfigureAwait(false);
				if (walletTransactions.IsNullOrEmpty())
				{
					Log.Message(LogLevel.Info, "No {0} withdrawals.", currency.Symbol);
					return; // no deposits
				}

				var existingWithdraws = await context.Withdraw.Where(x => x.CurrencyId == currency.Id && x.WithdrawStatus == WithdrawStatus.Complete).ToListAsync();
				foreach (var walletWithdraw in walletTransactions.OrderBy(x => x.Time))
				{
					var existingWithdraw = existingWithdraws.FirstOrDefault(x => x.Txid == walletWithdraw.Txid);
					if (existingWithdraw == null)
					{
						existingWithdraw.Confirmations = walletWithdraw.Confirmations;
					}
				}
				await context.SaveChangesAsync();
				Log.Message(LogLevel.Info, "Processing {0} withdrawals complete.", currency.Symbol);
			}
			catch (Exception ex)
			{
			}
		}

		private async Task SendWithdrawals(IDataContext context, Currency currency)
		{
			try
			{
				Log.Message(LogLevel.Info, "Sending pending {0} withdrawals...", currency.Symbol);
				if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline)
					return; // currency is skipped

				var connector = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
				var pendingWithdraws = await context.Withdraw.Where(x => x.CurrencyId == currency.Id && x.WithdrawStatus == WithdrawStatus.Pending).ToListAsync();
				foreach (var pendingWithdraw in pendingWithdraws)
				{
					pendingWithdraw.WithdrawStatus = WithdrawStatus.Processing;
				}
				await context.SaveChangesAsync();

				foreach (var pendingWithdraw in pendingWithdraws)
				{
					try
					{
						if(!await AuditUser(context, pendingWithdraw.CurrencyId, pendingWithdraw.UserId))
						{
							continue;
						}

						var balance = await context.Balance.FirstOrDefaultAsync(x => x.UserId == pendingWithdraw.UserId && x.CurrencyId == pendingWithdraw.CurrencyId);
						if (balance == null || balance.Total <= 0)
							continue;

						if (balance.Total < pendingWithdraw.Amount || balance.PendingWithdraw < pendingWithdraw.Amount || balance.PendingWithdraw > balance.Total)
							continue;

						decimal withdrawFee = GetWithdrawFee(currency, pendingWithdraw.Amount);
						decimal amountExcludingFees = pendingWithdraw.Amount - withdrawFee;
						var withdrawResult = await connector.SendToAddressAsync(pendingWithdraw.Address, amountExcludingFees);
						if (withdrawResult == null || string.IsNullOrEmpty(withdrawResult.Txid))
							continue;

						// Update the withdraw with the txid and set to completed
						pendingWithdraw.Txid = withdrawResult.Txid;
						await context.SaveChangesAsync();

						await AuditUser(context, pendingWithdraw.CurrencyId, pendingWithdraw.UserId);
					}
					catch (Exception ex)
					{

					}
				}
				Log.Message(LogLevel.Info, "Sending pending {0} withdrawals complete.", currency.Symbol);
			}
			catch (Exception ex)
			{
			}
		}

		private async Task<bool> AuditUser(IDataContext context, int currencyId, string userId)
		{
			Log.Message(LogLevel.Info, "Auditing user balance. UserId: {0}, CurrencyId: {1}", userId, currencyId);
			var result = await AuditService.AuditUserCurrencyAsync(context, userId, currencyId);
			Log.Message(LogLevel.Info, "Auditing user balance complete.");
			return result.Success;
		}




		private async Task<List<TransactionData>> GetTransactions(Currency currency, TransactionDataType type)
		{
			var deposits = GetWalletTransactions(currency, type, 5000);
			if (deposits != null)
			{
				return deposits;
			}
			await Task.Delay(1000).ConfigureAwait(false);
			deposits = GetWalletTransactions(currency, type, 10000);
			if (deposits != null)
			{
				return deposits;
			}
			await Task.Delay(1000).ConfigureAwait(false);
			deposits = GetWalletTransactions(currency, type, 60000);
			if (deposits != null)
			{
				return deposits;
			}
			return null;
		}

		private async Task<GetInfoData> GetInfo(Currency currency)
		{
			var info = GetInfo(currency, 5000);
			if (info != null)
			{
				return info;
			}
			await Task.Delay(1000).ConfigureAwait(false);
			info = GetInfo(currency, 10000);
			if (info != null)
			{
				return info;
			}
			await Task.Delay(1000).ConfigureAwait(false);
			info = GetInfo(currency, 60000);
			if (info != null)
			{
				return info;
			}
			return null;
		}

		private List<TransactionData> GetWalletTransactions(Currency currency, TransactionDataType type, int timeout)
		{
			try
			{
				var lastHash = type == TransactionDataType.Withdraw ? currency.LastWithdrawBlockHash : currency.LastBlockHash;
				var connector = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, timeout);
				var deposits = new List<TransactionData>(connector.GetTransactions(lastHash, type));
				return deposits.Where(x => x.Amount > 0).ToList();
			}
			catch (Exception ex)
			{
				Log.Exception("An exception occured querying wallet transactions. Currency: {0}", ex, currency.Symbol);
			}
			return null;
		}

		private GetInfoData GetInfo(Currency currency, int timeout)
		{
			try
			{
				var connector = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, timeout);
				return connector.GetInfo();
			}
			catch (Exception ex)
			{
				Log.Exception("An exception occured querying wallet information. Currency: {0}", ex, currency.Symbol);
			}
			return null;
		}

		private decimal GetWithdrawFee(Currency currency, decimal amount)
		{
			switch (currency.WithdrawFeeType)
			{
				case WithdrawFeeType.Normal:
					return currency.WithdrawFee;
				case WithdrawFeeType.Percent:
					return (amount / 100m) * currency.WithdrawFee;
				default:
					break;
			}
			return -1m;
		}
	}
}
