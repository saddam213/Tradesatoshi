using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.AuditService;
using System.Data.Entity;
using System.Diagnostics;

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
				var depositsConfirmed = deposits.Where(x => x.Status == DepositStatus.Confirmed).Sum(x => x.Amount);
				var depositsUnconfirmed = deposits.Where(x => x.Status == DepositStatus.UnConfirmed).Sum(x => x.Amount);

				var withdraws = context.Withdraw
					.Where(x => x.CurrencyId == currencyId && x.UserId == userId)
					.Select(x => new
					{
						Status = x.WithdrawStatus,
						Amount = (decimal?)x.Amount ?? 0m
					}).ToList();

				var withdrawPending = withdraws.Where(x => x.Status == WithdrawStatus.Pending || x.Status == WithdrawStatus.Processing || x.Status == WithdrawStatus.Unconfirmed).Sum(x => x.Amount);
				var withdrawConfirmed = withdraws.Where(x => x.Status == WithdrawStatus.Complete).Sum(x => x.Amount);

				var totalBuy = context.TradeHistory.Where(x => x.CurrencyId == currencyId && x.UserId == userId).Sum(x => (decimal?)x.Amount ?? 0m);
				var totalBuyFee = context.TradeHistory.Include(t => t.TradePair).Where(x => x.UserId == userId && x.TradePair.CurrencyId2 == currencyId).Sum(x => (decimal?)x.Amount ?? 0m);
				var totalBuyBase = context.TradeHistory.Where(x => x.CurrencyId == currencyId && x.ToUserId == userId && x.TradePair.CurrencyId2 == currencyId).Sum(x => ((decimal?)x.Amount ?? 0m) * ((decimal?)x.Rate ?? 0m));

				var totalSell = context.TradeHistory.Where(x => x.CurrencyId == currencyId && x.ToUserId == userId).Sum(x => (decimal?)x.Amount ?? 0m);
				var totalSellFee = context.TradeHistory.Include(t => t.TradePair).Where(x => x.ToUserId == userId && x.TradePair.CurrencyId2 == currencyId).Sum(x => (decimal?)x.Amount ?? 0m);
				var totalSellBase = context.TradeHistory.Include(t => t.TradePair).Where(x => x.UserId == userId && x.TradePair.CurrencyId2 == currencyId).Sum(x => ((decimal?)x.Amount ?? 0m) * ((decimal?)x.Rate ?? 0m));

				var heldForOrders = context.Trade.Include(t => t.TradePair).Where(x => x.UserId == userId && x.TradeType == TradeHistoryType.Sell && x.TradePair.CurrencyId1 == currencyId && (x.Status == TradeStatus.Pending || x.Status == TradeStatus.Partial)).Sum(x => (decimal?)x.Remaining ?? 0m);
				var heldForOrdersBase = context.Trade.Include(t => t.TradePair).Where(x => x.UserId == userId && x.TradeType == TradeHistoryType.Buy && x.TradePair.CurrencyId2 == currencyId && (x.Status == TradeStatus.Pending || x.Status == TradeStatus.Partial)).Sum(x => (((decimal?)x.Remaining ?? 0m) * ((decimal?)x.Rate ?? 0m)) + (decimal?)x.Fee ?? 0m);


				var totalIn = depositsConfirmed + depositsUnconfirmed + totalBuy + totalBuyBase;
				var totalOut = withdrawConfirmed + totalSell + totalSellBase + totalBuyFee + totalSellFee;
				var totalHeldForOrders = heldForOrders + heldForOrdersBase;
				var totalPendingWithdraw = withdrawPending;
				var totalUnconfirmed = depositsUnconfirmed;
				var total = totalIn - totalOut;


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
				balance.Unconfirmed = totalUnconfirmed;
				balance.HeldForTrades = totalHeldForOrders;
				balance.PendingWithdraw = totalPendingWithdraw;

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

				var totalBuy = await context.TradeHistory.Where(x => x.CurrencyId == currencyId && x.UserId == userId).DefaultIfEmpty().SumAsync(x => (decimal?)x.Amount ?? 0m);
				var totalBuyFee = await context.TradeHistory.Include(t => t.TradePair).Where(x => x.UserId == userId && x.TradePair.CurrencyId2 == currencyId).DefaultIfEmpty().SumAsync(x => (decimal?)x.Amount ?? 0m);
				var totalBuyBase = await context.TradeHistory.Where(x => x.CurrencyId == currencyId && x.ToUserId == userId && x.TradePair.CurrencyId2 == currencyId).DefaultIfEmpty().SumAsync(x => ((decimal?)x.Amount ?? 0m) * ((decimal?)x.Rate ?? 0m));

				var totalSell = await context.TradeHistory.Where(x => x.CurrencyId == currencyId && x.ToUserId == userId).DefaultIfEmpty().SumAsync(x => (decimal?)x.Amount ?? 0m);
				var totalSellFee = await context.TradeHistory.Include(t => t.TradePair).Where(x => x.ToUserId == userId && x.TradePair.CurrencyId2 == currencyId).DefaultIfEmpty().SumAsync(x => (decimal?)x.Amount ?? 0m);
				var totalSellBase = await context.TradeHistory.Include(t => t.TradePair).Where(x => x.UserId == userId && x.TradePair.CurrencyId2 == currencyId).DefaultIfEmpty().SumAsync(x => ((decimal?)x.Amount ?? 0m) * ((decimal?)x.Rate ?? 0m));

				var heldForOrders = await context.Trade.Include(t => t.TradePair).Where(x => x.UserId == userId && x.TradeType == TradeHistoryType.Sell && x.TradePair.CurrencyId1 == currencyId && (x.Status == TradeStatus.Pending || x.Status == TradeStatus.Partial)).DefaultIfEmpty().SumAsync(x => (decimal?)x.Remaining ?? 0m);
				var heldForOrdersBase = await context.Trade.Include(t => t.TradePair).Where(x => x.UserId == userId && x.TradeType == TradeHistoryType.Buy && x.TradePair.CurrencyId2 == currencyId && (x.Status == TradeStatus.Pending || x.Status == TradeStatus.Partial)).DefaultIfEmpty().SumAsync(x => (((decimal?)x.Remaining ?? 0m) * ((decimal?)x.Rate ?? 0m)) + (decimal?)x.Fee ?? 0m);


				var totalIn = depositsConfirmed + depositsUnconfirmed + totalBuy + totalBuyBase;
				var totalOut = withdrawConfirmed + totalSell + totalSellBase + totalBuyFee + totalSellFee;
				var totalHeldForOrders = heldForOrders + heldForOrdersBase;
				var totalPendingWithdraw = withdrawPending;
				var totalUnconfirmed = depositsUnconfirmed;
				var total = totalIn - totalOut;


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
				balance.Unconfirmed = totalUnconfirmed;
				balance.HeldForTrades = totalHeldForOrders;
				balance.PendingWithdraw = totalPendingWithdraw;

				await context.SaveChangesAsync();
				return new AuditResult(true);
			}
			catch (Exception ex)
			{
				return new AuditResult(false);
			}
		}


		public AuditResult AuditUserTradePair(IDataContext context, string userId, Common.Data.Entities.TradePair tradepair)
		{
			var result = AuditUserCurrency(context, userId, tradepair.CurrencyId1);
			if (!result.Success)
				return result;

			result = AuditUserCurrency(context, userId, tradepair.CurrencyId2);
			return result;
		}

		public async Task<AuditResult> AuditUserTradePairAsync(IDataContext context, string userId, Common.Data.Entities.TradePair tradepair)
		{
			var result = await AuditUserCurrencyAsync(context, userId, tradepair.CurrencyId1);
			if (!result.Success)
				return result;

			result = await AuditUserCurrencyAsync(context, userId, tradepair.CurrencyId2);
			return result;
		}
	}
}
