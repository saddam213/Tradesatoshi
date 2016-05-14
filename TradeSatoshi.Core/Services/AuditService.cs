using System;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.AuditService;
using System.Data.Entity;
using TradeSatoshi.Enums;
using TradeSatoshi.Common.Logging;

namespace TradeSatoshi.Core.Services
{
	public class AuditService : IAuditService
	{
		public AuditService() { }
		public AuditService(ILogger logger)
		{
			Logger = logger;
		}

		public ILogger Logger { get; set; }

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

				var withdraws = await context.Withdraw
					.Where(x => x.CurrencyId == currencyId && x.UserId == userId)
					.Select(x => new
					{
						Status = x.WithdrawStatus,
						Amount = (decimal?)x.Amount ?? 0m
					}).ToListAsync();

				var userTradeHistory = await context.TradeHistory
					.Where(x => (x.UserId == userId || x.ToUserId == userId) && (x.TradePair.CurrencyId1 == currencyId || x.TradePair.CurrencyId2 == currencyId))
					.Select(t => new
					{
						t.UserId,
						t.ToUserId,
						Fee = t.Fee,
						Amount = t.Amount,
						Rate = t.Rate,
						CurrencyId1 = t.TradePair.CurrencyId1,
						CurrencyId2 = t.TradePair.CurrencyId2
					}).ToListAsync();

				var userTrades = await context.Trade
					.Where(x => x.UserId == userId && (x.Status == TradeStatus.Pending || x.Status == TradeStatus.Partial) && (x.TradePair.CurrencyId1 == currencyId || x.TradePair.CurrencyId2 == currencyId))
					.Select(t => new
					{
						t.TradeType,
						Remaining = t.Remaining,
						Rate = t.Rate,
						Fee = t.Fee,
						t.TradePair.CurrencyId1,
						t.TradePair.CurrencyId2
					}).ToListAsync();

				var userTransfers = await context.TransferHistory
					.Where(x => x.CurrencyId == currencyId && (x.UserId == userId || x.ToUserId == userId))
					.Select(t => new
					{
						t.UserId,
						t.ToUserId,
						t.Amount
					}).ToListAsync();

				var depositsConfirmed = deposits.Where(x => x.Status == DepositStatus.Confirmed).Sum(x => x.Amount);
				var depositsUnconfirmed = deposits.Where(x => x.Status == DepositStatus.UnConfirmed).Sum(x => x.Amount);
				var withdrawPending = withdraws.Where(x => x.Status == WithdrawStatus.Pending || x.Status == WithdrawStatus.Processing || x.Status == WithdrawStatus.Unconfirmed).Sum(x => x.Amount);
				var withdrawConfirmed = withdraws.Where(x => x.Status == WithdrawStatus.Complete).Sum(x => x.Amount);
				var totalBuy = userTradeHistory.Where(x => x.UserId == userId && x.CurrencyId1 == currencyId).Sum(x => x.Amount);
				var totalBuyFee = userTradeHistory.Where(x => x.UserId == userId && x.CurrencyId2 == currencyId).Sum(x => x.Fee);
				var totalBuyBase = userTradeHistory.Where(x => x.ToUserId == userId && x.CurrencyId2 == currencyId).Sum(x => x.Amount * x.Rate);
				var totalSell = userTradeHistory.Where(x => x.ToUserId == userId && x.CurrencyId1 == currencyId).Sum(x => x.Amount);
				var totalSellFee = userTradeHistory.Where(x => x.ToUserId == userId && x.CurrencyId2 == currencyId).Sum(x => x.Fee);
				var totalSellBase = userTradeHistory.Where(x => x.UserId == userId && x.CurrencyId2 == currencyId).Sum(x => x.Amount * x.Rate);
				var heldForOrders = userTrades.Where(x => x.CurrencyId1 == currencyId && x.TradeType == TradeType.Sell).Sum(x => x.Remaining);
				var heldForOrdersBase = userTrades.Where(x => x.CurrencyId2 == currencyId && x.TradeType == TradeType.Buy).Sum(x => (x.Remaining * x.Rate) + x.Fee);
				var transferIn = userTransfers.Where(x => x.ToUserId == userId).Sum(x => x.Amount);
				var transferOut = userTransfers.Where(x => x.UserId == userId).Sum(x => x.Amount);

				// Sum sub totals
				var totalIn = depositsConfirmed + depositsUnconfirmed + totalBuy + totalBuyBase + transferIn;
				var totalOut = withdrawConfirmed + totalSell + totalSellBase + totalBuyFee + totalSellFee + transferOut;
				var totalHeldForOrders = heldForOrders + heldForOrdersBase;
				var totalPendingWithdraw = withdrawPending;
				var totalUnconfirmed = depositsUnconfirmed;
				var total = totalIn - totalOut;

				// Get or create user balance
				var balance = await context.Balance
				.Include(x => x.User)
				.Include(b => b.Currency)
				.FirstOrDefaultAsync(x => x.CurrencyId == currencyId && x.UserId == userId);
				if (balance == null)
				{
					var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
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
					Avaliable = balance.Avaliable,
					UserName = balance.User.UserName
				};
			}
			catch (Exception ex)
			{
				await Logger.Exception("AuditService", ex, $"Failed to audit user balance, User: {userId}, CurrencyId: {currencyId}");
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
