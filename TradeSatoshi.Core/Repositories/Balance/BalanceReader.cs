using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Data;

namespace TradeSatoshi.Core.Balance
{
	public class BalanceReader : IBalanceReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<BalanceModel> GetBalance(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = from currency in context.Currency.Where(c => c.Id == currencyId)
					from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currency.Id).DefaultIfEmpty()
					from address in context.Address.Where(a => a.UserId == userId && a.CurrencyId == currency.Id && a.IsActive).DefaultIfEmpty()
					where currency.IsEnabled
					select new BalanceModel
					{
						CurrencyId = currency.Id,
						Currency = currency.Name,
						Symbol = currency.Symbol,
						Address = address.AddressHash,
						HeldForTrades = (decimal?) balance.HeldForTrades ?? 0m,
						PendingWithdraw = (decimal?) balance.PendingWithdraw ?? 0m,
						Total = (decimal?) balance.Total ?? 0m,
						Unconfirmed = (decimal?) balance.Unconfirmed ?? 0m
					};
				return await query.FirstOrDefaultNoLockAsync();
			}
		}

		public async Task<List<BalanceModel>> GetBalances(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = from currency in context.Currency
					from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currency.Id).DefaultIfEmpty()
					from address in context.Address.Where(a => a.UserId == userId && a.CurrencyId == currency.Id && a.IsActive).DefaultIfEmpty()
					where currency.IsEnabled
					orderby currency.Name
					select new BalanceModel
					{
						CurrencyId = currency.Id,
						Currency = currency.Name,
						Symbol = currency.Symbol,
						Address = address.AddressHash,
						HeldForTrades = (decimal?) balance.HeldForTrades ?? 0m,
						PendingWithdraw = (decimal?) balance.PendingWithdraw ?? 0m,
						Total = (decimal?) balance.Total ?? 0m,
						Unconfirmed = (decimal?) balance.Unconfirmed ?? 0m
					};
				return await query.ToListNoLockAsync();
			}
		}
	}
}