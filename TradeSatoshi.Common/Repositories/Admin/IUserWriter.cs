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
		IWriterResult<bool> UpdateUser(UpdateUserModel model);
		Task<IWriterResult<bool>> UpdateUserAsync(UpdateUserModel model);

		IWriterResult<bool> AddUserRole(UserRoleModel model);
		Task<IWriterResult<bool>> AddUserRoleAsync(UserRoleModel model);

		IWriterResult<bool> RemoveUserRole(UserRoleModel model);
		Task<IWriterResult<bool>> RemoveUserRoleAsync(UserRoleModel model);
	}
}
