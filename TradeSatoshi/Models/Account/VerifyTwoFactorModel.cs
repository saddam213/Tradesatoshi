using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeSatoshi.Data.Entities;

namespace TradeSatoshi.Models.Account
{
	public class VerifyTwoFactorModel
	{
		public TwoFactorType TwoFactorType { get; set; }
		public TwoFactorComponentType TwoFactorComponentType { get; set; }
		public string Data { get; set; }

		public string UnlockSummary
		{
			get
			{
				switch (TwoFactorType)
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
}