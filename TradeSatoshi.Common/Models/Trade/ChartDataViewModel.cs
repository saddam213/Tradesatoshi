using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public class ChartDataViewModel
	{
		public ChartDataViewModel()
		{
			Candle = new List<decimal[]>();
			Volume = new List<decimal[]>();
		}

		public List<decimal[]> Candle { get; set; }
		public List<decimal[]> Volume { get; set; }
	}
}
