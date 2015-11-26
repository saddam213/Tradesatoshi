using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Data
{
	public enum TwoFactorTokenType : byte
	{
		EmailConfirm = 1,
		LockAccount = 1,
		WithdrawConfirm = 2,
		WithdrawCancel = 3,
	}
}
