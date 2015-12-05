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
	public interface IBalanceReader
	{
		BalanceModel GetBalance(string userId, int currencyId);
		List<BalanceModel> GetBalances(string userId);

		Task<BalanceModel> GetBalanceAsync(string userId, int currencyId);
		Task<List<BalanceModel>> GetBalancesAsync(string userId);

		DataTablesResponse GetBalanceDataTable(DataTablesModel model);
		DataTablesResponse GetUserBalanceDataTable(DataTablesModel model, string userId);
	}
}
