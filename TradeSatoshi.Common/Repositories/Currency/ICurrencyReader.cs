using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Common.Balance
{
	public interface ICurrencyReader
	{
		CurrencyModel GetCurrency(int currencyId);
		List<CurrencyModel> GetCurrencies();

		Task<CurrencyModel> GetCurrencyAsync(int currencyId);
		Task<List<CurrencyModel>> GetCurrenciesAsync();

		DataTablesResponse GetCurrencyDataTable(DataTablesModel model);
	}
}
