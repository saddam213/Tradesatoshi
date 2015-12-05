using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Data.DataContext;
using System.Data.Entity;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Core.Heplers;
using TradeSatoshi.Common;
using System.Threading;
using System.Security.Claims;
using System.Security.Permissions;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Balance;

namespace TradeSatoshi.Core.Balance
{
	public class BalanceReader : IBalanceReader
	{
		public IDataContext DataContext { get; set; }

		public BalanceModel GetBalance(string userId, int currencyId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = from currency in context.Currency.Where(c => c.Id == currencyId)
							from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currency.Id)
							.DefaultIfEmpty()
							select new BalanceModel
							{
								Currency = currency.Name,
								Symbol = currency.Symbol,
								HeldForTrades = (decimal?)balance.HeldForTrades ?? 0m,
								PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0m,
								Total = (decimal?)balance.Total ?? 0m,
								Unconfirmed = (decimal?)balance.Unconfirmed ?? 0m
							};
				return query.FirstOrDefault();
			}
		}

		public List<BalanceModel> GetBalances(string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = from currency in context.Currency
							from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currency.Id)
							.DefaultIfEmpty()
							select new BalanceModel
							{
								Currency = currency.Name,
								Symbol = currency.Symbol,
								HeldForTrades = (decimal?)balance.HeldForTrades ?? 0m,
								PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0m,
								Total = (decimal?)balance.Total ?? 0m,
								Unconfirmed = (decimal?)balance.Unconfirmed ?? 0m
							};
				return query.ToList();
			}
		}

		public async Task<BalanceModel> GetBalanceAsync(string userId, int currencyId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = from currency in context.Currency.Where(c => c.Id == currencyId)
							from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currency.Id).DefaultIfEmpty()
							select new BalanceModel
							{
								Currency = currency.Name,
								Symbol = currency.Symbol,
								HeldForTrades = (decimal?)balance.HeldForTrades ?? 0m,
								PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0m,
								Total = (decimal?)balance.Total ?? 0m,
								Unconfirmed = (decimal?)balance.Unconfirmed ?? 0m
							};
				return await query.FirstOrDefaultAsync();
			}
		}

		public async Task<List<BalanceModel>> GetBalancesAsync(string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = from currency in context.Currency
							from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currency.Id).DefaultIfEmpty()
							select new BalanceModel
							{
								Currency = currency.Name,
								Symbol = currency.Symbol,
								HeldForTrades = (decimal?)balance.HeldForTrades ?? 0m,
								PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0m,
								Total = (decimal?)balance.Total ?? 0m,
								Unconfirmed = (decimal?)balance.Unconfirmed ?? 0m
							};
				return await query.ToListAsync();
			}
		}

		public DataTablesResponse GetBalanceDataTable(DataTablesModel model)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = from currency in context.Currency
							from balance in context.Balance.Where(b => b.CurrencyId == currency.Id).DefaultIfEmpty()
							select new BalanceModel
							{
								Currency = currency.Name,
								Symbol = currency.Symbol,
								HeldForTrades = (decimal?)balance.HeldForTrades ?? 0m,
								PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0m,
								Total = (decimal?)balance.Total ?? 0m,
								Unconfirmed = (decimal?)balance.Unconfirmed ?? 0m
							};
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetUserBalanceDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = from currency in context.Currency
							from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currency.Id).DefaultIfEmpty()
							select new BalanceModel
							{
								Currency = currency.Name,
								Symbol = currency.Symbol,
								HeldForTrades = (decimal?)balance.HeldForTrades ?? 0m,
								PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0m,
								Total = (decimal?)balance.Total ?? 0m,
								Unconfirmed = (decimal?)balance.Unconfirmed ?? 0m
							};
				return query.GetDataTableResult(model);
			}
		}
	}
}
