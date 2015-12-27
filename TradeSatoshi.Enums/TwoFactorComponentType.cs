using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Enums
{
	public enum TwoFactorComponentType
	{
		[Display(Name = "Login")]
		Login = 0,
		[Display(Name = "Withdraw")]
		Withdraw = 1,
		[Display(Name = "Transfer")]
		Transfer = 2,
	}
}
