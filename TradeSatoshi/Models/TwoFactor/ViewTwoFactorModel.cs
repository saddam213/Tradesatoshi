using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeSatoshi.Common;

namespace TradeSatoshi.Models.TwoFactor
{
	public class ViewTwoFactorModel
	{
		public TwoFactorType Type { get; set; }
		public TwoFactorComponentType ComponentType { get; set; }
	}
}