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
		Task<IWriterResult<bool>> UpdateUser(UpdateUserModel model);
		
		Task<IWriterResult<bool>> AddUserRole(UserRoleModel model);

		Task<IWriterResult<bool>> RemoveUserRole(UserRoleModel model);
	}
}
