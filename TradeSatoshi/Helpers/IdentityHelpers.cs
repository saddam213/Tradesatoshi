using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Helpers
{
	public static class IdentityHelpers
	{
		public static bool IsInRole(this IPrincipal principal, params SecurityRole[] roles)
		{
			if (principal == null || roles == null)
				return false;

			return roles.Any(role => principal.IsInRole(role.ToString()));
		}
	}

	public class AuthorizeSecurityRoleAttribute : AuthorizeAttribute
	{
		public AuthorizeSecurityRoleAttribute(params SecurityRole[] roles)
		{
			if (roles != null)
				Roles = string.Join(", ", roles.Select(x => x.ToString()));
		}
	}
}