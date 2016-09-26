using System;
using System.Threading.Tasks;
using System.Data.Entity;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.AuditService;
using TradeSatoshi.Enums;
using System.Linq;
using TradeSatoshi.Common.Services.WalletService;

namespace TradeSatoshi.Core.Withdraw
{
	public class WithdrawWriter : IWithdrawWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IAuditService AuditService { get; set; }
		public IWalletService WalletService { get; set; }

		public async Task<IWriterResult<int>> CreateWithdraw(string userId, CreateWithdrawModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currency = await context.Currency.FirstOrDefaultAsync(x => x.Id == model.CurrencyId);
				if (currency == null || !currency.IsEnabled || currency.Status != CurrencyStatus.OK)
					return WriterResult<int>.ErrorResult("Currency not found or is currently disabled.");

				var isValidAddress = await WalletService.ValidateAddress(model.Address, currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
				if (!isValidAddress)
					return WriterResult<int>.ErrorResult($"Invalid {currency.Symbol} address.");

				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
				if (user == null || !user.IsWithdrawEnabled)
					return WriterResult<int>.ErrorResult("Your withdrawals are currently disabled.");

				var auditResult = await AuditService.AuditUserCurrency(context, userId, model.CurrencyId);
				if (!auditResult.Success || model.Amount > auditResult.Avaliable)
					return WriterResult<int>.ErrorResult("Insufficient funds.");

				var newWithdraw = new Entity.Withdraw
				{
					IsApi = false,
					TimeStamp = DateTime.UtcNow,
					TwoFactorToken = model.ConfirmationToken,
					Address = model.Address,
					CurrencyId = model.CurrencyId,
					Amount = model.Amount,
					Fee = currency.WithdrawFee,
					WithdrawType = WithdrawType.Normal,
					WithdrawStatus = WithdrawStatus.Unconfirmed,
					UserId = userId
				};

				context.Withdraw.Add(newWithdraw);
				await context.SaveChangesAsync();
				await AuditService.AuditUserCurrency(context, userId, model.CurrencyId);
				return WriterResult<int>.SuccessResult(newWithdraw.Id);
			}
		}

		public async Task<IWriterResult<int>> CreateApiWithdraw(string userId, string currency, string address, decimal amount)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currencyEntity = await context.Currency.Where(w => w.Symbol == currency).FirstOrDefaultNoLockAsync();
				if (currencyEntity == null)
					return WriterResult<int>.ErrorResult("Currency not found.");

				var isValidAddress = await WalletService.ValidateAddress(address, currencyEntity.WalletHost, currencyEntity.WalletPort, currencyEntity.WalletUser, currencyEntity.WalletPass);
				if (!isValidAddress)
					return WriterResult<int>.ErrorResult($"Invalid {currencyEntity.Symbol} address.");

				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
				if (user == null || !user.IsWithdrawEnabled)
					return WriterResult<int>.ErrorResult("Your withdrawals are currently disabled.");

				var auditResult = await AuditService.AuditUserCurrency(context, userId, currencyEntity.Id);
				if (!auditResult.Success || amount > auditResult.Avaliable)
					return WriterResult<int>.ErrorResult("Failed to audit balance.");

				var newWithdraw = new Entity.Withdraw
				{
					IsApi = true,
					TimeStamp = DateTime.UtcNow,
					TwoFactorToken = string.Empty,
					Address = address,
					CurrencyId = currencyEntity.Id,
					Amount = amount,
					Fee = currencyEntity.WithdrawFee,
					WithdrawType = WithdrawType.Normal,
					WithdrawStatus = WithdrawStatus.Pending,
					UserId = userId
				};

				context.Withdraw.Add(newWithdraw);
				await context.SaveChangesAsync();
				await AuditService.AuditUserCurrency(context, userId, currencyEntity.Id);
				return WriterResult<int>.SuccessResult(newWithdraw.Id);
			}
		}

		public async Task<IWriterResult<bool>> ConfirmWithdraw(string userId, int withdrawId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var withdraw = await context.Withdraw
					.Include(x => x.User)
					.Include(x => x.Currency)
					.FirstOrDefaultAsync(x => x.Id == withdrawId && x.UserId == userId && x.WithdrawStatus == WithdrawStatus.Unconfirmed);
				if (withdraw == null || withdraw.WithdrawStatus != WithdrawStatus.Unconfirmed)
					return WriterResult<bool>.ErrorResult("Withdraw #{0} not found or is already confirmed.", withdrawId);
				if (!withdraw.User.IsWithdrawEnabled)
					return WriterResult<bool>.ErrorResult("Your withdrawals are currently disabled.");

				withdraw.WithdrawStatus = WithdrawStatus.Pending;
				await context.SaveChangesAsync();

				await AuditService.AuditUserCurrency(context, userId, withdraw.CurrencyId);
				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<IWriterResult<bool>> CancelWithdraw(string userId, int withdrawId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var withdraw = await context.Withdraw
					.Include(x => x.User)
					.Include(x => x.Currency)
					.FirstOrDefaultAsync(x => x.Id == withdrawId && x.UserId == userId && x.WithdrawStatus == WithdrawStatus.Unconfirmed);
				if (withdraw == null || withdraw.WithdrawStatus != WithdrawStatus.Unconfirmed)
					return WriterResult<bool>.ErrorResult("Withdraw #{0} not found or is already canceled.", withdrawId);
				if (!withdraw.User.IsWithdrawEnabled)
					return WriterResult<bool>.ErrorResult("Your withdrawals are currently disabled.");


				withdraw.WithdrawStatus = WithdrawStatus.Canceled;
				await context.SaveChangesAsync();

				await AuditService.AuditUserCurrency(context, userId, withdraw.CurrencyId);
				return WriterResult<bool>.SuccessResult();
			}
		}


	}
}