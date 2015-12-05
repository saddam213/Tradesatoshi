﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common
{

	public enum CurrencyStatus
	{
		OK = 0,
		Maintenance = 1,
		Offline = 5
	}

	public enum WithdrawFeeType
	{
		Normal = 0,
		Percent = 1
	}

	public enum WithdrawType
	{
		Normal = 0,
		Other = 10,
	}

	public enum WithdrawStatus 
	{
		Pending = 0,
		Processing = 1,
		Complete = 2,
		Error = 3,
		Unconfirmed = 4,
		Canceled = 5
	}

	public enum DepositType
	{
		Normal = 0,
		Other = 10
	}

	public enum DepositStatus
	{
		UnConfirmed = 0,
		Confirmed = 1,
		Error = 2,
	}
}