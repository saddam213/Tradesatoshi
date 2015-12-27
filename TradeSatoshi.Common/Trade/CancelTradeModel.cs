using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Trade
{
	public class CancelTradeModel : ITradeItem
	{
		public string UserId { get; set; }
		public int TradeId { get; set; }
		public bool IsApi { get; set; }

		public CancelTradeType CancelType { get; set; }
		public int TradePairId { get; set; }
	}
}
