using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public class ChartDataModel
	{
		public ChartDataModel() { }
		public ChartDataModel(long time, decimal open, decimal high, decimal low, decimal close, decimal volume)
		{
			Timestamp = time;
			Open = open;
			High = high;
			Low = low;
			Close = close;
			Volume = volume;
		}
		public long Timestamp { get; set; }
		public decimal Open { get; set; }
		public decimal High { get; set; }
		public decimal Low { get; set; }
		public decimal Close { get; set; }
		public decimal Volume { get; set; }
	}
}
