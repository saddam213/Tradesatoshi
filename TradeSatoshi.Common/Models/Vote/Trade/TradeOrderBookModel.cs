using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public class TradeOrderBookModel
	{
		public decimal Rate { get; set; }
		public decimal Total { get; set; }
		//public int OrderCount { get; set; }

		
		//public decimal SumTotal { get; set; }
	}
}
