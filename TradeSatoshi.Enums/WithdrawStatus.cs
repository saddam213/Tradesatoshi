using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Enums
{
	public enum WithdrawStatus
	{
		Pending = 0,
		Processing = 1,
		Complete = 2,
		Error = 3,
		Unconfirmed = 4,
		Canceled = 5
	}
}
