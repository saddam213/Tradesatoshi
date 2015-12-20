using TradeSatoshi.Common;

namespace TradeSatoshi.Web.Helpers
{
	public static class TwoFactorHelpers
	{
		public static string GetTwoFactorTypeSummary(this TwoFactorType twoFactorType)
		{
			switch (twoFactorType)
			{
				case TwoFactorType.None:
					break;
				case TwoFactorType.EmailCode:
					return "Please enter email code.";
				case TwoFactorType.GoogleCode:
				return "Please enter Google Authenticator code.";
				case TwoFactorType.PinCode:
					return "Please enter pin code.";
				default:
					break;
			}
			return string.Empty;
		}
	}
}