using System.Collections.Generic;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Repositories.Api;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Common.Exchange
{
	public class ExchangeSummaryModel
	{
		public List<TradePairModel> TradePairs { get; set; }
		public List<ApiMarketSummary> MarketSummary { get; set; }
	}
}