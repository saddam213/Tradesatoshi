using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Trade
{
	public class TradeOpenOrderModel
	{
		public int Id { get; set; }

		public TradeType TradeType { get; set; }

		public decimal Rate { get; set; }

		public decimal Amount { get; set; }

		public decimal Remaining { get; set; }

		public DateTime Timestamp { get; set; }
	}
}
