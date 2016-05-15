using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Enums
{
	public enum EmailType
	{
		Logon = 0,
		FailedLogon = 1,
		PasswordLockout = 2,
		UserLockout = 3,
		Registration = 4,
		TwoFactorLogin = 5,
		TwoFactorUnlockCode = 6,
		PasswordReset = 7,
		TwoFactorWithdraw = 8,
		WithdrawConfirmation = 9,
	}

}
