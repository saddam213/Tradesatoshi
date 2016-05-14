using System.ComponentModel.DataAnnotations;
using TradeSatoshi.Enums;

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
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }
		public bool IsApi { get; set; }
		public string Market { get; set; }
	}
}