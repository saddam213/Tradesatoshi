using TradeSatoshi.Common;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.TwoFactor
{
	public class RemoveTwoFactorModel
	{
		public TwoFactorType Type { get; set; }
		public TwoFactorComponentType ComponentType { get; set; }
		public string Data { get; set; }
	}
}