using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

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