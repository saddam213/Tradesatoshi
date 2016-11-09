using System.ComponentModel.DataAnnotations;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.TwoFactor
{
	public class RemoveTwoFactorModel
	{
		public TwoFactorType Type { get; set; }
		public TwoFactorComponentType ComponentType { get; set; }

		[Display(Name = "Code")]
		public string Data { get; set; }
	}
}