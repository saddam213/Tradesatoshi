using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Repositories.Trade
{
	public static class TradeCacheKeys
	{
		public static string GetOpenBuyOrdersKey(int tradePairId)
		{
			return $"trade-getopenbuyorders-{tradePairId}";
		}

		public static string GetOpenSellOrdersKey(int tradePairId)
		{
			return $"trade-getopensellorders-{tradePairId}";
		}

		public static string GetTradeHistoryKey(int tradePairId)
		{
			return $"trade-gettradehistory-{tradePairId}";
		}

		public static string GetTradePairChartKey(int tradePairId)
		{
			return $"trade-gettradepairchart-{tradePairId}";
		}

		public static string GetTradePairDepthKey(int tradePairId)
		{
			return $"trade-gettradepairdepth-{tradePairId}";
		}
	}
}
