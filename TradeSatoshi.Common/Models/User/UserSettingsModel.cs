using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Common.User
{
	public class UserSettingsModel : ITradeSidebarModel
	{
		public UserSettingsModel()
		{

		}

		public List<BalanceModel> Balances { get; set; }
		public List<TradePairModel> TradePairs { get; set; }
		public UserSecurityModel SecurityModel { get; set; }
		public UserProfileModel UserProfile { get; set; }
	}
}