using System.Collections.Generic;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Common.Exchange
{
	public class ExchangeModel : ITradeSidebarModel
	{
		public ExchangeModel()
		{
			Balances = new List<BalanceModel>();
			TradePairs = new List<TradePairModel>();
		}

		public string TradePair { get; set; }
		public List<BalanceModel> Balances { get; set; }
		public List<TradePairModel> TradePairs { get; set; }
	}
}