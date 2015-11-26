using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeSatoshi.Data.Entities;

namespace TradeSatoshi.Models.TwoFactor
{
	public class RemoveTwoFactorModel
	{
		public TwoFactorType Type { get; set; }
		public TwoFactorComponentType ComponentType { get; set; }
		public string Data { get; set; }
	}
}