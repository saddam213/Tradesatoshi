using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Core.Admin
{
	public interface IUserReader
	{
		Task<UserModel> GetUser(string userId);
		Task<UpdateUserModel> GetUserUpdate(string userId);
		Task<DataTablesResponse> GetUserDataTable(DataTablesModel model);
		Task<DataTablesResponse> GetLogonDataTable(DataTablesModel model);
		Task<DataTablesResponse> GetRolesDataTable(DataTablesModel model, SecurityRole securityRole);
	}
}