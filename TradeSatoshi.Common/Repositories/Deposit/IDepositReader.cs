using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

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