using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Repositories.Api
{
	public static class ApiCacheKeys
	{
		public static string GetCurrenciesKey()
		{
			return $"api-getcurrencies";
		}
		public static string GetTickerKey(string market)
		{
			return $"api-getticker-{market}";
    }

		public static string GetMarketHistoryKey(string market, int count)
		{
			return $"api-getmarkethistory-{market}-{count}";
		}

		public static string GetMarketSummaryKey(string market)
		{
			return $"api-getmarketsummary-{market}";
		}

		public static string GetMarketSummariesKey()
		{
			return $"api-getmarketsummaries";
		}

		public static string GetOrderBookKey(string market, string type, int depth)
		{
			return $"api-getorderbook-{market}-{type}-{depth}";
		}
	}
}
