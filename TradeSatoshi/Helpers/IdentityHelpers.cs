using Microsoft.AspNet.Identity;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Web.Helpers
{
	public static class IdentityHelpers
	{

		public static string Id(this IPrincipal principal)
		{
			if (principal == null || principal.Identity == null)
				return string.Empty;

			return principal.Identity.GetUserId();
		}

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