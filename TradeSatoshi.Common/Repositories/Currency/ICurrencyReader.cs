using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Common.Currency
{
	public interface ICurrencyReader
	{
		Task<CurrencyModel> GetCurrency(int currencyId);
		Task<List<CurrencyModel>> GetCurrencies();
		Task<UpdateCurrencyModel> GetCurrencyUpdate(int currencyId);
		DataTablesResponse GetCurrencyDataTable(DataTablesModel model);

		Task<List<CurrencyStatusModel>> GetCurrencyStatus();
	}
}
