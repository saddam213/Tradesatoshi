using System;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Trade
{
	public class TradeHistoryModel
	{
		public int Id { get; set; }
		public string TradePair { get; set; }
		public TradeType TradeHistoryType { get; set; }
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public decimal Fee { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsApi { get; set; }
	}

	public class TradeHistoryDataTableModel
	{
		public DateTime Timestamp { get; set; }
		public TradeType TradeHistoryType { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }
	}
}