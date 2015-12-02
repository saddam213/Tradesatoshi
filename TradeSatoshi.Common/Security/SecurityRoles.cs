using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Security
{
	public enum SecurityRole
	{
		Standard = 0,
		Administrator = 1,
		Moderator = 2
	}

	public static class SecurityRoles
	{
		public const string Standard = "Standard";
		public const string Administrator = "Administrator";
		public const string Moderator = "Moderator";
	}
}
