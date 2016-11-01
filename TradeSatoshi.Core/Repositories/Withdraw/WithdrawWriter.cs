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
using TradeSatoshi.Base.Queueing;
using TradeSatoshi.Core.Services;
using TradeSatoshi.Common.Logging;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Core.Logger;
using TradeSatoshi.Common.Services.WithdrawService;

namespace TradeSatoshi.Core.Withdraw
{
	
	public class WithdrawWriter : IWithdrawWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IAuditService AuditService { get; set; }
		public IWithdrawService WithdrawService { get; set; }

		public async Task<IWriterResult<int>> CreateWithdraw(string userId, CreateWithdrawModel model)
		{
			return await WithdrawService.QueueWithdraw(new CreateWithdraw
			{
				IsApi = false,
				UserId = userId,
				Address = model.Address,
				Amount = model.Amount,
				ConfirmationToken = model.ConfirmationToken,
				CurrencyId = model.CurrencyId
			});
		}

		public async Task<IWriterResult<int>> CreateApiWithdraw(string userId, string currency, string address, decimal amount)
		{
			int currencyId = 0;
			using (var context = DataContextFactory.CreateContext())
			{
				var currencyEntity = await context.Currency.Where(w => w.Symbol == currency).FirstOrDefaultNoLockAsync();
				if (currencyEntity == null)
					return WriterResult<int>.ErrorResult("Currency not found.");

				currencyId = currencyEntity.Id;
			}

			return await WithdrawService.QueueWithdraw(new CreateWithdraw
			{
				IsApi = true,
				UserId = userId,
				Address = address,
				Amount = amount,
				ConfirmationToken = "",
				CurrencyId = currencyId
			});
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