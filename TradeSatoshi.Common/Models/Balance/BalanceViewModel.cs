using System.Collections.Generic;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Common.Balance
{
	public class BalanceViewModel : ITradeSidebarModel
	{
		public BalanceViewModel()
		{
			Balances = new List<BalanceModel>();
			TradePairs = new List<TradePairModel>();
		}

		public List<BalanceModel> Balances { get; set; }
		public List<TradePairModel> TradePairs { get; set; }
	}
}