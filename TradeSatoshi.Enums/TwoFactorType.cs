using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Enums
{
	public enum TwoFactorType
	{
		None = 0,
		[Display(Name = "Email")]
		EmailCode = 1,
		[Display(Name = "Google Authenticator")]
		GoogleCode = 2,
		[Display(Name = "Pin Number")]
		PinCode = 3,
	}
}
