using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Common.Withdraw
{
	public interface IWithdrawReader
	{
		CreateWithdrawModel GetCreateWithdraw(string userId, int currencyId);
		Task<CreateWithdrawModel> GetCreateWithdrawAsync(string userId, int currencyId);

		List<WithdrawModel> GetWithdrawals(string userId);
		List<WithdrawModel> GetWithdrawals(string userId, int currencyId);

		Task<List<WithdrawModel>> GetWithdrawalsAsync(string userId);
		Task<List<WithdrawModel>> GetWithdrawalsAsync(string userId, int currencyId);

		DataTablesResponse GetWithdrawDataTable(DataTablesModel model);
		DataTablesResponse GetUserWithdrawDataTable(DataTablesModel model, string userId);
	}
}
