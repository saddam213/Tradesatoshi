using System.Collections.Generic;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Common.History
{
	public class HistoryViewModel : ITradeSidebarModel
	{
		public HistoryViewModel()
		{
			Balances = new List<BalanceModel>();
			TradePairs = new List<TradePairModel>();
		}
	
		public List<BalanceModel> Balances { get; set; }
		public List<TradePairModel> TradePairs { get; set; }

		public string Area { get; set; }

		public string Section { get; set; }

		public string Currency { get; set; }
	}
}