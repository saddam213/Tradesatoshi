using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeSatoshi.Common.Trade;

namespace TradeSatoshi.Models.Exchange
{
	public class TradePairExchangeModel
	{
		public int TradePairId { get; set; }
		public string Symbol { get; set; }
		public string BaseSymbol { get; set; }

		public CreateTradeModel BuyModel { get; set; }
		public CreateTradeModel SellModel { get; set; }

		public decimal Balance { get; set; }
		public decimal BaseBalance { get; set; }
	}
}