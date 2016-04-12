using System.Collections.Generic;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Common
{
	public interface ITradeSidebarModel
	{
		List<BalanceModel> Balances { get; set; }
		List<TradePairModel> TradePairs { get; set; }
	}
}