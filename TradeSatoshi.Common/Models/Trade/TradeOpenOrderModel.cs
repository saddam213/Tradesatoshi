using System;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Trade
{
	public class TradeOpenOrderModel
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public TradeType TradeType { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }
		public decimal Remaining { get; set; }
		public TradeStatus Status { get; set; }
	}
}