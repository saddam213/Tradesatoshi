using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Common.Admin
{
	public class UserRoleModel
	{
		public string UserName { get; set; }
		public SecurityRole SecurityRole { get; set; }
	}
}
