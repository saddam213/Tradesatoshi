using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Security
{
	public interface ITwoFactorEntry
	{
		string Data { get; set; }
		TwoFactorType TwoFactorType { get; set; }
		TwoFactorComponentType TwoFactorComponentType { get; set; }
	}
}