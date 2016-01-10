using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Web.Attributes
{
	public class AuthorizeSecurityRoleAttribute : System.Web.Mvc.AuthorizeAttribute
	{
		public AuthorizeSecurityRoleAttribute(params SecurityRole[] roles)
		{
			if (roles != null)
				Roles = string.Join(", ", roles.Select(x => x.ToString()));
		}
	}

	public class AuthorizeSignalRSecurityRoleAttribute : Microsoft.AspNet.SignalR.AuthorizeAttribute
	{
		public AuthorizeSignalRSecurityRoleAttribute(params SecurityRole[] roles)
		{
			if (roles != null)
				Roles = string.Join(", ", roles.Select(x => x.ToString()));
		}
	}
}