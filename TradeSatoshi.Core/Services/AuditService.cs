using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.AuditService;
using System.Data.Entity;

namespace TradeSatoshi.Core.Services
{
	public class AuditService : IAuditService
	{
		public AuditResult AuditUserCurrency(IDataContext context, string userId, int currencyId)
		{
			try
			{
				var deposits = context.Deposit
						.Where(x => x.CurrencyId == currencyId && x.UserId == userId)
						.Select(x => new
						{
							Status = x.DepositStatus,
							Amount = (decimal?)x.Amount ?? 0m
						}).ToList();
				var withdraws = context.Withdraw
					.Where(x => x.CurrencyId == currencyId && x.UserId == userId)
					.Select(x => new
					{
						Status = x.WithdrawStatus,
						Amount = (decimal?)x.Amount ?? 0m
					}).ToList();

				var depositsConfirmed = deposits.Where(x => x.Status == DepositStatus.Confirmed).Sum(x => x.Amount);
				var depositsUnconfirmed = deposits.Where(x => x.Status == DepositStatus.UnConfirmed).Sum(x => x.Amount);

				var withdrawPending = withdraws.Where(x => x.Status == WithdrawStatus.Pending || x.Status == WithdrawStatus.Processing || x.Status == WithdrawStatus.Unconfirmed).Sum(x => x.Amount);
				var withdrawConfirmed = withdraws.Where(x => x.Status == WithdrawStatus.Complete).Sum(x => x.Amount);

				var heldForTrades = 0;
				var pendingWithdraw = withdrawPending;
				var unconfirmed = depositsUnconfirmed;
				var total = (depositsConfirmed + depositsUnconfirmed) - (withdrawConfirmed);


				var balance = context.Balance.FirstOrDefault(x => x.CurrencyId == currencyId && x.UserId == userId);
				if (balance == null)
				{
					context.Balance.Add(new Common.Data.Entities.Balance
					{
						UserId = userId,
						CurrencyId = currencyId,
					});
				}

				balance.Total = total;
				balance.Unconfirmed = unconfirmed;
				balance.HeldForTrades = heldForTrades;
				balance.PendingWithdraw = pendingWithdraw;

				context.SaveChanges();
				return new AuditResult(true);
			}
			catch (Exception)
			{
				return new AuditResult(false);
			}
		}

		public async Task<AuditResult> AuditUserCurrencyAsync(IDataContext context, string userId, int currencyId)
		{
			try
			{
				var deposits = await context.Deposit
						.Where(x => x.CurrencyId == currencyId && x.UserId == userId)
						.Select(x => new
						{
							Status = x.DepositStatus,
							Amount = (decimal?)x.Amount ?? 0m
						}).ToListAsync();
				var withdraws = await context.Withdraw
					.Where(x => x.CurrencyId == currencyId && x.UserId == userId)
					.Select(x => new
					{
						Status = x.WithdrawStatus,
						Amount = (decimal?)x.Amount ?? 0m
					}).ToListAsync();

				var depositsConfirmed = deposits.Where(x => x.Status == DepositStatus.Confirmed).Sum(x => x.Amount);
				var depositsUnconfirmed = deposits.Where(x => x.Status == DepositStatus.UnConfirmed).Sum(x => x.Amount);

				var withdrawPending = withdraws.Where(x => x.Status == WithdrawStatus.Pending || x.Status == WithdrawStatus.Processing || x.Status == WithdrawStatus.Unconfirmed).Sum(x => x.Amount);
				var withdrawConfirmed = withdraws.Where(x => x.Status == WithdrawStatus.Complete).Sum(x => x.Amount);

				var heldForTrades = 0;
				var pendingWithdraw = withdrawPending;
				var unconfirmed = depositsUnconfirmed;
				var total = (depositsConfirmed + depositsUnconfirmed) - (withdrawConfirmed);


				var balance = await context.Balance.FirstOrDefaultAsync(x => x.CurrencyId == currencyId && x.UserId == userId);
				if (balance == null)
				{
					context.Balance.Add(new Common.Data.Entities.Balance
					{
						UserId = userId,
						CurrencyId = currencyId,
					});
				}

				balance.Total = total;
				balance.Unconfirmed = unconfirmed;
				balance.HeldForTrades = heldForTrades;
				balance.PendingWithdraw = pendingWithdraw;

				await context.SaveChangesAsync();
				return new AuditResult(true);
			}
			catch (Exception)
			{
				return new AuditResult(false);
			}
		}
	}
}
