using System.Collections.Generic;

namespace TradeSatoshi.Common.Trade
{
	public class CreateTradeResponse : ITradeResponse
	{
		public CreateTradeResponse()
		{
			FilledTrades = new List<int>();
		}

		public CreateTradeResponse(string error)
			: base()
		{
			Error = error;
		}

		public int? TradeId { get; set; }
		public List<int> FilledTrades { get; set; }

		public string Error { get; set; }

		public bool HasError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}

		public void AddFilledTrade(int tradeId)
		{
			FilledTrades.Add(tradeId);
		}
	}
}