using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

	public enum EmailType
	{
		Logon = 0,
		FailedLogon = 1,
		PasswordLockout = 2,
		UserLockout = 3,
		Registration = 4,
		TwoFactorLogin = 5,
		TwoFactorUnlockCode = 6,
		PasswordReset,
		TwoFactorWithdraw,
		WithdrawConfirmation,
	}

	public enum NotificationType
	{
		Info,
		Success,
		Warning,
		Error
	}

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

	public enum TwoFactorComponentType
	{
		[Display(Name = "Login")]
		Login = 0,
		[Display(Name = "Withdraw")]
		Withdraw = 1,
	}

	public enum TradeStatus
	{
		Pending = 0,
		Complete = 1,
		Partial = 2,
		Canceled = 3
	}
	public enum TradeHistoryType
	{
		Buy = 0,
		Sell = 1
	}

	public enum TradePairStatus
	{
		OK = 0,
		Closing = 1,
		Paused = 2,
		Closed = 3
	}
}
