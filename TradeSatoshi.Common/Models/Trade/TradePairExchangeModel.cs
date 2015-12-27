using TradeSatoshi.Common.Trade;

namespace TradeSatoshi.Common.Exchange
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