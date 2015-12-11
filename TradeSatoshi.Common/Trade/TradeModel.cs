using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public class TradeModel
	{
		public int Id { get; set; }
		public string TradePair { get; set; }
		public TradeHistoryType TradeType { get; set; }
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public decimal Fee { get; set; }
		public decimal Remaining { get; set; }
		public TradeStatus Status { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsApi { get; set; }
	}
}
