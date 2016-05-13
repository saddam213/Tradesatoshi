using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Enums
{
	public enum TradeCancelType
	{
		Single = 0,
		Market = 1,
		MarketBuys = 2,
		MarketSells = 3,
		AllBuys = 4,
		AllSells = 5,
		All = 6
	}
}
