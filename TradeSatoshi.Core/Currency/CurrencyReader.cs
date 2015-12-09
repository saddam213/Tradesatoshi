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
using TradeSatoshi.Common.Data;

namespace TradeSatoshi.Core.Currency
{
	public class CurrencyReader : ICurrencyReader
	{
		public IDataContext DataContext { get; set; }

		public CurrencyModel GetCurrency(int currencyId)
		{
			using (var context = DataContext.CreateContext())
			{
				var currency = context.Currency.Find(currencyId);
				if (currency == null)
					return null;

				return new CurrencyModel
				{
					Name = currency.Name,
					Symbol = currency.Symbol,
					Status = currency.Status
				};
			}
		}

		public List<CurrencyModel> GetCurrencies()
		{
			using (var context = DataContext.CreateContext())
			{
				return context.Currency.Select(currency =>
					new CurrencyModel
					{
						Name = currency.Name,
						Symbol = currency.Symbol,
						Status = currency.Status
					}).ToList();
			}
		}

		public async Task<CurrencyModel> GetCurrencyAsync(int currencyId)
		{
			using (var context = DataContext.CreateContext())
			{
				var currency = await context.Currency.FindAsync(currencyId);
				if (currency == null)
					return null;

				return new CurrencyModel
				{
					Name = currency.Name,
					Symbol = currency.Symbol,
					Status = currency.Status
				};
			}
		}

		public async Task<List<CurrencyModel>> GetCurrenciesAsync()
		{
			using (var context = DataContext.CreateContext())
			{
				return await context.Currency.Select(currency =>
					new CurrencyModel
					{
						Name = currency.Name,
						Symbol = currency.Symbol,
						Status = currency.Status
					}).ToListAsync();
			}
		}

		public DataTablesResponse GetCurrencyDataTable(DataTablesModel model)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Currency.Select(currency =>
						new CurrencyModel
						{
							Name = currency.Name,
							Symbol = currency.Symbol,
							Status = currency.Status
						});
				return query.GetDataTableResult(model);
			}
		}
	}
}
