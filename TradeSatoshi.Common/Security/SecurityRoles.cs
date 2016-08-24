namespace TradeSatoshi.Common.Security
{
	public enum SecurityRole
	{
		Standard = 0,
		Administrator = 1,
		Moderator1 = 2,
		Moderator2 = 3
		//VerifiedUser = 4,
		//UnverifiedUser = 5
	}

	public static class SecurityRoles
	{
		public const string Standard = "Standard";
		public const string Administrator = "Administrator";
		public const string Moderator1 = "Moderator1";
		public const string Moderator2 = "Moderator2";
		//public const string VerifiedUser = "VerifiedUser";
		//public const string UnverifiedUser = "UnverifiedUser";
	}
}