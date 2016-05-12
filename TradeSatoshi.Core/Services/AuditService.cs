using System;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.AuditService;
using System.Data.Entity;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Services
{
	public class AuditService : IAuditService
	{
		public async Task<AuditCurrencyResult> AuditUserCurrency(IDataContext context, string userId, int currencyId)
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
				var depositsConfirmed = deposits.Where(x => x.Status == DepositStatus.Confirmed).Sum(x => x.Amount);
				var depositsUnconfirmed = deposits.Where(x => x.Status == DepositStatus.UnConfirmed).Sum(x => x.Amount);

				var withdraws = await context.Withdraw
					.Where(x => x.CurrencyId == currencyId && x.UserId == userId)
					.Select(x => new
					{
						Status = x.WithdrawStatus,
						Amount = (decimal?)x.Amount ?? 0m
					}).ToListAsync();

				var withdrawPending = withdraws.Where(x => x.Status == WithdrawStatus.Pending || x.Status == WithdrawStatus.Processing || x.Status == WithdrawStatus.Unconfirmed).Sum(x => x.Amount);
				var withdrawConfirmed = withdraws.Where(x => x.Status == WithdrawStatus.Complete).Sum(x => x.Amount);

				var totalBuy = await context.TradeHistory
					.Where(x => x.CurrencyId == currencyId && x.UserId == userId)
					.DefaultIfEmpty()
					.SumAsync(x => (decimal?)x.Amount ?? 0m);

				var totalBuyFee = await context.TradeHistory
					.Include(t => t.TradePair)
					.Where(x => x.UserId == userId && x.TradePair.CurrencyId2 == currencyId)
					.DefaultIfEmpty()
					.SumAsync(x => (decimal?)x.Fee ?? 0m);

				var totalBuyBase = await context.TradeHistory
					.Where(x => x.ToUserId == userId && x.TradePair.CurrencyId2 == currencyId)
					.DefaultIfEmpty()
					.SumAsync(x => ((decimal?)x.Amount ?? 0m) * ((decimal?)x.Rate ?? 0m));

				var totalSell = await context.TradeHistory
					.Where(x => x.CurrencyId == currencyId && x.ToUserId == userId)
					.DefaultIfEmpty()
					.SumAsync(x => (decimal?)x.Amount ?? 0m);

				var totalSellFee = await context.TradeHistory
					.Include(t => t.TradePair)
					.Where(x => x.ToUserId == userId && x.TradePair.CurrencyId2 == currencyId)
					.DefaultIfEmpty()
					.SumAsync(x => (decimal?)x.Fee ?? 0m);

				var totalSellBase = await context.TradeHistory
					.Include(t => t.TradePair)
					.Where(x => x.UserId == userId && x.TradePair.CurrencyId2 == currencyId)
					.DefaultIfEmpty()
					.SumAsync(x => ((decimal?)x.Amount ?? 0m) * ((decimal?)x.Rate ?? 0m));

				var heldForOrders = await context.Trade
					.Include(t => t.TradePair)
					.Where(x => x.UserId == userId && x.TradeType == TradeType.Sell && x.TradePair.CurrencyId1 == currencyId && (x.Status == TradeStatus.Pending || x.Status == TradeStatus.Partial))
					.DefaultIfEmpty()
					.SumAsync(x => (decimal?)x.Remaining ?? 0m);

				var heldForOrdersBase = await context.Trade
					.Include(t => t.TradePair)
					.Where(x => x.UserId == userId && x.TradeType == TradeType.Buy && x.TradePair.CurrencyId2 == currencyId && (x.Status == TradeStatus.Pending || x.Status == TradeStatus.Partial))
					.DefaultIfEmpty()
					.SumAsync(x => (((decimal?)x.Remaining ?? 0m) * ((decimal?)x.Rate ?? 0m)) + (decimal?)x.Fee ?? 0m);

				var transferIn = await context.TransferHistory
					.Where(x => x.ToUserId == userId && x.CurrencyId == currencyId)
					.DefaultIfEmpty()
					.SumAsync(x => (decimal?)x.Amount ?? 0m);

				var transferOut = await context.TransferHistory
					.Where(x => x.UserId == userId && x.CurrencyId == currencyId)
					.DefaultIfEmpty()
					.SumAsync(x => (decimal?)x.Amount ?? 0m);

				// Sum sub totals
				var totalIn = depositsConfirmed + depositsUnconfirmed + totalBuy + totalBuyBase + transferIn;
				var totalOut = withdrawConfirmed + totalSell + totalSellBase + totalBuyFee + totalSellFee + transferOut;
				var totalHeldForOrders = heldForOrders + heldForOrdersBase;
				var totalPendingWithdraw = withdrawPending;
				var totalUnconfirmed = depositsUnconfirmed;
				var total = totalIn - totalOut;

				// Get or create user balance
				var balance = await context.Balance.Include(b => b.Currency).FirstOrDefaultAsync(x => x.CurrencyId == currencyId && x.UserId == userId);
				if (balance == null)
				{
					var currency = await context.Currency.FirstOrDefaultAsync(x => x.Id == currencyId);
					balance = context.Balance.Add(new Entity.Balance
					{
						UserId = userId,
						CurrencyId = currencyId,
					});
				}

				// Update user balance
				balance.Total = total;
				balance.Unconfirmed = totalUnconfirmed;
				balance.HeldForTrades = totalHeldForOrders;
				balance.PendingWithdraw = totalPendingWithdraw;

				// Save changes
				await context.SaveChangesAsync();
				return new AuditCurrencyResult
				{
					Success = true,
					UserId = balance.UserId,
					CurrencyId = balance.CurrencyId,
					Symbol = balance.Currency.Symbol,
					Total = balance.Total,
					Unconfirmed = balance.Unconfirmed,
					HeldForTrades = balance.HeldForTrades,
					PendingWithdraw = balance.PendingWithdraw,
					Avaliable = balance.Avaliable
				};
			}
			catch (Exception)
			{
				return new AuditCurrencyResult(false);
			}
		}

		public async Task<AuditTradePairResult> AuditUserTradePair(IDataContext context, string userId, Entity.TradePair tradepair)
		{
			var result = await AuditUserCurrency(context, userId, tradepair.CurrencyId1);
			var baseResult = await AuditUserCurrency(context, userId, tradepair.CurrencyId2);
			if (!result.Success || !baseResult.Success)
				return new AuditTradePairResult(false);

			return new AuditTradePairResult
			{
				Success = true,
				Currency = result,
				BaseCurrency = baseResult
			};
		}
	}
}
