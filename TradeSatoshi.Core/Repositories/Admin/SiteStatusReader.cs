using System;
using System.Collections.Generic;
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
				var last24 = DateTime.UtcNow.AddHours(-24);
				var deposits = await context.Deposit.CountNoLockAsync();
				var deposits24 = await context.Deposit.CountNoLockAsync(x => x.TimeStamp > last24);

				var withdrawals = await context.Withdraw.CountNoLockAsync(x => x.WithdrawStatus != Enums.WithdrawStatus.Canceled);
				var withdrawals24 = await context.Withdraw.CountNoLockAsync(x => x.WithdrawStatus != Enums.WithdrawStatus.Canceled && x.TimeStamp > last24);

				var transfers = await context.TransferHistory.CountNoLockAsync();
				var transfers24 = await context.TransferHistory.CountNoLockAsync(x => x.Timestamp > last24);

				var orders = await context.Trade.CountNoLockAsync(x => x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending);
				var orders24 = await context.Trade.CountNoLockAsync(x => x.Status == Enums.TradeStatus.Partial || x.Status == Enums.TradeStatus.Pending && x.Timestamp > last24);

				var trades = await context.TradeHistory.CountNoLockAsync();
				var trades24 = await context.TradeHistory.CountNoLockAsync(x => x.Timestamp > last24);

				var logons = await context.UserLogons.CountNoLockAsync();
				var logons24 = await context.UserLogons.CountNoLockAsync(x => x.Timestamp > last24);

				return new SiteStatusModel
				{
					Deposits = deposits,
					Deposits24 = deposits24,
					Withdrawals = withdrawals,
					Withdrawals24 = withdrawals24,
					Transfers = transfers,
					Transfers24 = transfers24,
					Orders = orders,
					Orders24 = orders24,
					Trades = trades,
					Trades24 = trades24,
					Logons = logons,
					Logons24 = logons24
				};
			}
		}
	}
}
