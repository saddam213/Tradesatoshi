using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public interface ITradeItem
	{
		bool IsBuy { get; set; }
		string UserId { get; set; }
		int TradePairId { get; set; }
		decimal Rate { get; set; }
		decimal Amount { get; set; }
		bool IsApi { get; set; }
	}
}
