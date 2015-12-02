using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Admin
{
	public interface IUserWriter
	{
		IWriterResult UpdateUser(UpdateUserModel model);
		Task<IWriterResult> UpdateUserAsync(UpdateUserModel model);

		IWriterResult AddUserRole(UserRoleModel model);
		Task<IWriterResult> AddUserRoleAsync(UserRoleModel model);

		IWriterResult RemoveUserRole(UserRoleModel model);
		Task<IWriterResult> RemoveUserRoleAsync(UserRoleModel model);
	}
}
