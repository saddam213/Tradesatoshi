using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public class TradePairInfoModel
	{
		public int TradePairId { get; set; }
		public string Symbol { get; set; }
		public string BaseSymbol { get; set; }

		public decimal Fee { get; set; }
		public decimal MinTrade { get; set; }

		public decimal Balance { get; set; }
		public decimal BaseBalance { get; set; }
	}
}
