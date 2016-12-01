using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Base.Queueing;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Logging;
using TradeSatoshi.Common.Services.AuditService;
using TradeSatoshi.Common.Services.WalletService;
using TradeSatoshi.Common.Services.WithdrawService;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Core.Logger;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Services
{
	public class WithdrawService : IWithdrawService
	{
		private static readonly ProcessorQueue<CreateWithdraw, IWriterResult<int>> WithdrawProcessor = new ProcessorQueue<CreateWithdraw, IWriterResult<int>>(WithdrawService.Processor());
		private static Func<CreateWithdraw, Task<IWriterResult<int>>> Processor()
		{
			var service = new WithdrawService();
			return service.ProcessWithdraw;
		}

		public ILogger Log { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IAuditService AuditService { get; set; }
		public IWalletService WalletService { get; set; }

		public WithdrawService()
		{
			DataContextFactory = new DataContextFactory();
			Log = new DatabaseLogger(DataContextFactory);
			AuditService = new AuditService(Log);
			WalletService = new WalletService();
		}

		private async Task<IWriterResult<int>> ProcessWithdraw(CreateWithdraw model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currency = await context.Currency.FirstOrDefaultAsync(x => x.Id == model.CurrencyId);
				if (currency == null || !currency.IsEnabled || currency.Status != CurrencyStatus.OK)
					return WriterResult<int>.ErrorResult("Currency not found or is currently disabled.");

				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
				if (user == null || !user.IsWithdrawEnabled)
					return WriterResult<int>.ErrorResult("Your withdrawals are currently disabled.");

				var auditResult = await AuditService.AuditUserCurrency(context, model.UserId, model.CurrencyId);
				if (!auditResult.Success || model.Amount > auditResult.Avaliable)
					return WriterResult<int>.ErrorResult("Insufficient funds.");

				if (model.Amount < currency.MinWithdraw || model.Amount > currency.MaxWithdraw)
					return WriterResult<int>.ErrorResult("Withdrawal amount must be between {0} and {1} {2}", currency.MinWithdraw, currency.MaxWithdraw, currency.Symbol);

				var isValidAddress = await WalletService.ValidateAddress(model.Address, currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
				if (!isValidAddress)
					return WriterResult<int>.ErrorResult($"Invalid {currency.Symbol} address.");

				var newWithdraw = new Entity.Withdraw
				{
					IsApi = model.IsApi,
					TimeStamp = DateTime.UtcNow,
					TwoFactorToken = model.ConfirmationToken,
					Address = model.Address,
					CurrencyId = model.CurrencyId,
					Amount = Math.Max(0, model.Amount),
					Fee = currency.WithdrawFee,
					WithdrawType = WithdrawType.Normal,
					WithdrawStatus = model.IsApi
					 ? WithdrawStatus.Pending
					 : WithdrawStatus.Unconfirmed,
					UserId = model.UserId
				};

				context.Withdraw.Add(newWithdraw);
				await context.SaveChangesAsync();
				await AuditService.AuditUserCurrency(context, model.UserId, model.CurrencyId);
				return WriterResult<int>.SuccessResult(newWithdraw.Id);
			}
		}

		public async Task<IWriterResult<int>> QueueWithdraw(CreateWithdraw withdraw)
		{
			return await WithdrawProcessor.QueueItem(withdraw).ConfigureAwait(false);
		}
	}
}
