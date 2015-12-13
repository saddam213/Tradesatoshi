using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public class CreateTradeModel : ITradeItem
	{
		public string UserId { get; set; }
		public int TradePairId { get; set; }
		public string Symbol { get; set; }
		public string BaseSymbol { get; set; }
		public decimal Fee { get; set; }
		public decimal MinTrade { get; set; }
		public TradeType TradeType { get; set; }

		[Range(typeof(decimal), "0.00000001", "200000000")]
		public decimal Rate { get; set; }

		[Range(typeof(decimal), "0.00000001", "200000000")]
		public decimal Amount { get; set; }
		
		public bool IsApi { get; set; }
	}
}
