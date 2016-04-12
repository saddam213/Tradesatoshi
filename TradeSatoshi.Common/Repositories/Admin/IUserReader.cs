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
		DataTablesResponse GetUserDataTable(DataTablesModel model);
		DataTablesResponse GetLogonDataTable(DataTablesModel model);
		DataTablesResponse GetRolesDataTable(DataTablesModel model, SecurityRole securityRole);
	}
}