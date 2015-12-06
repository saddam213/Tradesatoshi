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
using TradeSatoshi.Data.Entities;
using TradeSatoshi.Base.Extensions;
using System.Data.Entity;
using TradeSatoshi.Common.Services.NotificationService;
using TradeSatoshi.Core.Services;

namespace TradeSatoshi.WalletService.Implementation
{
	public class WalletTracker
	{
		private CancellationToken _cancelToken;
		private bool _isRunning = false;
		private bool _isEnabled = false;
		private int _pollPeriod = 300;
		public INotificationService NotificationService { get; set; }

		public WalletTracker(CancellationToken cancelToken)
		{
			_isRunning = true;
			_isEnabled = true;
			_cancelToken = cancelToken;
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
					await ProcessInfo();
					await ProcessDeposits();
					await ProcessWithdrawals();
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
			using (var context = new DataContext())
			{
				var currencies = await context.Currency.Where(x => x.IsEnabled).ToListAsync();
				foreach (var currency in currencies)
				{
					if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline)
						continue;

					var currencyInfo = await GetInfo(currency).ConfigureAwait(false);
					if (currencyInfo == null)
						continue;

					currency.Balance = currencyInfo.Balance;
					currency.Block = currencyInfo.Blocks;
					currency.Connections = currencyInfo.Connections;
					currency.Errors = currencyInfo.Errors;
					currency.Version = currencyInfo.Version;
				}
				await context.SaveChangesAsync();
			}
		}

		private async Task ProcessDeposits()
		{
			using (var context = new DataContext())
			{
				var userIds = await context.Users.Select(x => x.Id).ToListAsync();
				foreach (var currency in context.Currency.Where(x => x.IsEnabled))
				{
					if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline)
						continue; // currency is skipped

					var walletTransactions = await GetTransactions(currency, TransactionDataType.Deposit).ConfigureAwait(false);
					if (walletTransactions.IsNullOrEmpty())
						continue; // no new deposits

					var existingDeposits = await context.Deposit.Include(d => d.Currency).Where(x => x.CurrencyId == currency.Id).ToListAsync();
					foreach (var walletDeposit in walletTransactions.OrderBy(x => x.Time))
					{
						var userId = userIds.FirstOrDefault(id => id == walletDeposit.Account);
						if (string.IsNullOrEmpty(userId))
							continue; // user not found

						var existingDeposit = existingDeposits.FirstOrDefault(x => x.Txid == walletDeposit.Txid && x.UserId == userId);
						if (existingDeposit == null)
						{
							// deposit does not exist;
							var newDeposit = new Deposit
							{
								Amount = walletDeposit.Amount,
								Confirmations = walletDeposit.Confirmations,
								DepositType = DepositType.Normal,
								TimeStamp = DateTime.UtcNow,
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
								Message = string.Format("{0} Deposit #{1}, {2:F8} {3}", newDeposit.DepositStatus, existingDeposit.Id, existingDeposit.Amount, existingDeposit.Currency.Symbol),
								Type = NotificationType.Info
							});
							continue;
						}
						else
						{
							// deposit  exists;
							if (existingDeposit.DepositStatus == DepositStatus.Confirmed)
								continue; // its already confirmed

							existingDeposit.Confirmations = walletDeposit.Confirmations;
							if (existingDeposit.Confirmations >= currency.MinConfirmations)
							{
								existingDeposit.DepositStatus = DepositStatus.Confirmed;
							}

							await context.SaveChangesAsync();

							if (existingDeposit.DepositStatus == DepositStatus.Confirmed)
							{
								await AuditUser(context, currency.Id, userId);
								await NotificationService.SendUserNotificationAsync(userId, new Notification
								{
									Title = "Deposit Confirmed",
									Message = string.Format("Deposit #{0}, {1:F8} {2} has now been confirmed", existingDeposit.Id, existingDeposit.Amount, existingDeposit.Currency.Symbol),
									Type = NotificationType.Info
								});
							}
						}
					}
				}
			}
		}


		private async Task ProcessWithdrawals()
		{
			using (var context = new DataContext())
			{
				foreach (var currency in context.Currency.Where(x => x.IsEnabled))
				{
					if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline)
						continue;

					var walletTransactions = await GetTransactions(currency, TransactionDataType.Withdraw).ConfigureAwait(false);
					if (walletTransactions.IsNullOrEmpty())
						continue;


				}
			}
		}




		private async Task AuditUser(IDataContext context, int currencyId, string userId)
		{
			var depositsConfirmed = await context.Deposit.Where(x => x.DepositStatus == DepositStatus.Confirmed && x.CurrencyId == currencyId && x.UserId == userId).SumAsync(x => x.Amount);
			var depositsUnconfirmed = await context.Deposit.Where(x => x.DepositStatus == DepositStatus.UnConfirmed && x.CurrencyId == currencyId && x.UserId == userId).SumAsync(x => x.Amount);

			var balance = await context.Balance.FirstOrDefaultAsync(x => x.CurrencyId == currencyId && x.UserId == userId);
			if (balance == null)
			{
				context.Balance.Add(new Balance
				{
					UserId = userId,
					CurrencyId = currencyId,
					HeldForTrades = 0,
					PendingWithdraw = 0,
					Unconfirmed = depositsConfirmed,
					Total = depositsConfirmed + depositsUnconfirmed
				});
				await context.SaveChangesAsync();
				return;
			}
			balance.HeldForTrades = 0;
			balance.PendingWithdraw = 0;
			balance.Unconfirmed = depositsConfirmed;
			balance.Total = depositsConfirmed + depositsUnconfirmed;
			await context.SaveChangesAsync();
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
				var connector = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, timeout);
				var deposits = new List<TransactionData>(connector.GetTransactions(currency.LastBlockHash, type));
				return deposits.Where(x => x.Amount > 0).ToList();
			}
			catch (Exception)
			{
				return null;
			}
		}

		private GetInfoData GetInfo(Currency currency, int timeout)
		{
			try
			{
				var connector = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, timeout);
				return connector.GetInfo();
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
