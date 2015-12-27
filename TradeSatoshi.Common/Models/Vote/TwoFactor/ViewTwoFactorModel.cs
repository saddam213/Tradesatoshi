using TradeSatoshi.Common;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.TwoFactor
{
	public class ViewTwoFactorModel
	{
		public TwoFactorType Type { get; set; }
		public TwoFactorComponentType ComponentType { get; set; }
	}
}