﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.EmailService
{
	public enum EmailType
	{
		Logon = 0,
		FailedLogon = 1,
		PasswordLockout = 2,
		UserLockout = 2,
		Registration = 4,
	}
}