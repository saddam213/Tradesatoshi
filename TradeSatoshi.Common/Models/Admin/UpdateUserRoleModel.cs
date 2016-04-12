using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Common.Admin
{
	public class UpdateUserRoleModel
	{
		public string ButtonName { get; set; }
		public string ActionMethod { get; set; }
		public string Description { get; set; }

		public string UserName { get; set; }
		public SecurityRole SecurityRole { get; set; }
	}
}