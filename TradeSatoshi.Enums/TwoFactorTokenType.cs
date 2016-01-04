using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Enums
{
	public enum TwoFactorTokenType
	{
		EmailConfirm = 0,
		LockAccount = 1,
		WithdrawConfirm = 2,
		WithdrawCancel = 3,
	}
}
