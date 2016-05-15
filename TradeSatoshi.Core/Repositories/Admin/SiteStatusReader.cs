using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Repositories.Admin;

namespace TradeSatoshi.Core.Repositories.Admin
{
	public class SiteStatusReader : ISiteStatusReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<SiteStatusModel> GetSiteStatus()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var last24 = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
				var lastMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
				var lastYear = new DateTime(DateTime.UtcNow.Year, 1, 1);
				var data = new List<SiteStatisticModel>
				{
					new SiteStatisticModel
					{
						Name = "Deposits",
						AllTime = await context.Deposit.CountNoLockAsync(),
						Today = await context.Deposit.CountNoLockAsync(x => x.TimeStamp > last24),
						Month = await context.Deposit.CountNoLockAsync(x => x.TimeStamp > lastMonth),
						Year = await context.Deposit.CountNoLockAsync(x => x.TimeStamp > lastYear),
					},
					new SiteStatisticModel
					{
						Name = "Withdrawals",
						AllTime = await context.Withdraw.CountNoLockAsync(x => x.WithdrawStatus != Enums.WithdrawStatus.Canceled),
						Today = await context.Withdraw.CountNoLockAsync(x => x.WithdrawStatus != Enums.WithdrawStatus.Canceled && x.TimeStamp > last24),
						Month = await context.Withdraw.CountNoLockAsync(x => x.WithdrawStatus != Enums.WithdrawStatus.Canceled && x.TimeStamp > lastMonth),
						Year = await context.Withdraw.CountNoLockAsync(x => x.WithdrawStatus != Enums.WithdrawStatus.Canceled && x.TimeStamp > lastYear),
					},
					new SiteStatisticModel
					{
						Name = "Transfers",
						AllTime = await context.TransferHistory.CountNoLockAsync(),
						Today = await context.TransferHistory.CountNoLockAsync(x => x.Timestamp > last24),
						Month = await context.TransferHistory.CountNoLockAsync(x => x.Timestamp > lastMonth),
						Year = await context.TransferHistory.CountNoLockAsync(x => x.Timestamp > lastYear),
					},
					new SiteStatisticModel
					{
						Name = "Open Orders",
						AllTime = await context.Trade.CountNoLockAsync(x => x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending),
						Today = await context.Trade.CountNoLockAsync(x => x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending && x.Timestamp > last24),
						Month = await context.Trade.CountNoLockAsync(x => x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending && x.Timestamp > lastMonth),
						Year = await context.Trade.CountNoLockAsync(x => x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending && x.Timestamp > lastYear),
					},
					new SiteStatisticModel
					{
						Name = "Completed Trades",
						AllTime = await context.TradeHistory.CountNoLockAsync(),
						Today = await context.TradeHistory.CountNoLockAsync(x => x.Timestamp > last24),
						Month = await context.TradeHistory.CountNoLockAsync(x => x.Timestamp > lastMonth),
						Year = await context.TradeHistory.CountNoLockAsync(x => x.Timestamp > lastYear),
					},
					new SiteStatisticModel
					{
						Name = "New Users",
						AllTime = await context.Users.CountNoLockAsync(),
						Today = await context.Users.CountNoLockAsync(x => x.RegisterDate > last24),
						Month = await context.Users.CountNoLockAsync(x => x.RegisterDate > lastMonth),
						Year = await context.Users.CountNoLockAsync(x => x.RegisterDate > lastYear),
					},
					new SiteStatisticModel
					{
						Name = "User Logons",
						AllTime = await context.UserLogons.CountNoLockAsync(),
						Today = await context.UserLogons.CountNoLockAsync(x => x.Timestamp > last24),
						Month = await context.UserLogons.CountNoLockAsync(x => x.Timestamp > lastMonth),
						Year = await context.UserLogons.CountNoLockAsync(x => x.Timestamp > lastYear),
					}
				};

				var balances = await context.Balance
					.GroupBy(x => x.CurrencyId)
					.Select(x => new
					{
						CurrencyId = x.Key,
						Total = x.Sum(b => (decimal?)b.Total ?? 0m)
					}).ToListNoLockAsync();

				var deposits = await context.Deposit
					.GroupBy(x => x.CurrencyId)
					.Select(x => new
					{
						CurrencyId = x.Key,
						Total = x.Sum(b => (decimal?)b.Amount ?? 0m)
					}).ToListNoLockAsync();

				var withdrawals = await context.Withdraw
					.GroupBy(x => x.CurrencyId)
					.Select(x => new
					{
						CurrencyId = x.Key,
						Total = x.Sum(b => (decimal?)b.Amount ?? 0m)
					}).ToListNoLockAsync();

				var profitStats = new List<SiteProfitModel>();
				foreach (var currency in await context.Currency.ToListNoLockAsync())
				{
					profitStats.Add(new SiteProfitModel
					{
						Name = currency.Symbol,
						Balance = currency.Balance,
						ColdBalance = currency.ColdBalance,
						UserBalance = balances.Where(x => x.CurrencyId == currency.Id).Select(x => x.Total).FirstOrDefault(),
						TotalDeposit = deposits.Where(x => x.CurrencyId == currency.Id).Select(x => x.Total).FirstOrDefault(),
						TotalWithdraw = withdrawals.Where(x => x.CurrencyId == currency.Id).Select(x => x.Total).FirstOrDefault()
					});
				}


				return new SiteStatusModel
				{
					Statistic = data,
					Profit = profitStats
				};
			}
		}
	}
}
