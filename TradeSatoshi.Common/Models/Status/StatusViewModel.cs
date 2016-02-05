﻿using System.Collections.Generic;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Common.Status
{
	public class StatusViewModel : ITradeSidebarModel
	{
		public StatusViewModel()
		{
			Balances = new List<BalanceModel>();
			TradePairs = new List<TradePairModel>();
		}
	
		public List<BalanceModel> Balances { get; set; }
		public List<TradePairModel> TradePairs { get; set; }
	}
}