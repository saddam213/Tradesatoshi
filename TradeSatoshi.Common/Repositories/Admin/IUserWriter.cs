using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
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