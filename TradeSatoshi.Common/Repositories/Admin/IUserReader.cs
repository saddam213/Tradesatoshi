using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Core.Admin
{
	public interface IUserReader
	{
		UserModel GetUser(string userId);
		Task<UserModel> GetUserAsync(string userId);
		UpdateUserModel GetUserUpdate(string userId);
		Task<UpdateUserModel> GetUserUpdateAsync(string userId);

		DataTablesResponse GetUserDataTable(DataTablesModel model);
		DataTablesResponse GetLogonDataTable(DataTablesModel model);
		DataTablesResponse GetRolesDataTable(DataTablesModel model, SecurityRole securityRole);
	}
		

}
