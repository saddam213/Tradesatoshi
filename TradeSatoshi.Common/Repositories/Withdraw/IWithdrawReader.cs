using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Common.Withdraw
{
	public interface IWithdrawReader
	{
		Task<CreateWithdrawModel> GetCreateWithdraw(string userId, int currencyId);

		Task<List<WithdrawModel>> GetWithdrawals(string userId);
		Task<List<WithdrawModel>> GetWithdrawals(string userId, int currencyId);

		DataTablesResponse GetWithdrawDataTable(DataTablesModel model);
		DataTablesResponse GetUserWithdrawDataTable(DataTablesModel model, string userId);
	}
}