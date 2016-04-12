using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Common.Admin
{
	public class UserRoleModel
	{
		public string UserName { get; set; }
		public SecurityRole SecurityRole { get; set; }
	}
}