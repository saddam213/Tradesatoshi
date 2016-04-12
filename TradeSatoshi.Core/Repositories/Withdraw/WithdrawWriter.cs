using System;
using System.Threading.Tasks;
using System.Data.Entity;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.AuditService;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Withdraw
{
	public class WithdrawWriter : IWithdrawWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IAuditService AuditService { get; set; }

		public async Task<IWriterResult<int>> CreateWithdraw(string userId, CreateWithdrawModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var auditResult = await AuditService.AuditUserCurrency(context, userId, model.CurrencyId);
				if (!auditResult.Success)
					return WriterResult<int>.ErrorResult("Failed to audit balance.");

				var balance = await context.Balance.Include(x => x.Currency).FirstOrDefaultAsync(x => x.UserId == userId && x.CurrencyId == model.CurrencyId);
				if (balance == null || model.Amount > balance.Avaliable)
					return WriterResult<int>.ErrorResult("Insufficient funds.");

				var newWithdraw = new Entity.Withdraw
				{
					IsApi = false,
					TimeStamp = DateTime.UtcNow,
					TwoFactorToken = model.ConfirmationToken,
					Address = model.Address,
					CurrencyId = model.CurrencyId,
					Amount = model.Amount,
					Fee = balance.Currency.WithdrawFee,
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

		public async Task<IWriterResult<bool>> ConfirmWithdraw(string userId, int withdrawId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var withdraw = await context.Withdraw
					.Include(x => x.Currency)
					.FirstOrDefaultAsync(x => x.Id == withdrawId && x.UserId == userId && x.WithdrawStatus == WithdrawStatus.Unconfirmed);
				if (withdraw == null || withdraw.WithdrawStatus != WithdrawStatus.Unconfirmed)
					return WriterResult<bool>.ErrorResult("Withdraw #{0} not found or is already confirmed.", withdrawId);

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
					.Include(x => x.Currency)
					.FirstOrDefaultAsync(x => x.Id == withdrawId && x.UserId == userId && x.WithdrawStatus == WithdrawStatus.Unconfirmed);
				if (withdraw == null || withdraw.WithdrawStatus != WithdrawStatus.Unconfirmed)
					return WriterResult<bool>.ErrorResult("Withdraw #{0} not found or is already canceled.", withdrawId);

				withdraw.WithdrawStatus = WithdrawStatus.Canceled;
				await context.SaveChangesAsync();

				await AuditService.AuditUserCurrency(context, userId, withdraw.CurrencyId);
				return WriterResult<bool>.SuccessResult();
			}
		}
	}
}