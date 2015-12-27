using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public interface ITradeItem
	{
		string UserId { get; set; }
		bool IsApi { get; set; }
	}
}
