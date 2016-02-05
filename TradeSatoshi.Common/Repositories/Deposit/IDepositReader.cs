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
		Task<List<DepositModel>> GetDeposits(string userId);
		Task<List<DepositModel>> GetDeposits(string userId, int currencyId);

		DataTablesResponse GetDepositDataTable(DataTablesModel model);
		DataTablesResponse GetUserDepositDataTable(DataTablesModel model, string userId);
	}
}
