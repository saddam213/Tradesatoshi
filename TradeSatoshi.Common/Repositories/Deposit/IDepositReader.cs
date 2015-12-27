using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Common.Deposit
{
	public interface IDepositReader
	{
		List<DepositModel> GetDeposits(string userId);
		List<DepositModel> GetDeposits(string userId, int currencyId);

		Task<List<DepositModel>> GetDepositsAsync(string userId);
		Task<List<DepositModel>> GetDepositsAsync(string userId, int currencyId);

		DataTablesResponse GetDepositDataTable(DataTablesModel model);
		DataTablesResponse GetUserDepositDataTable(DataTablesModel model, string userId);
	}
}
