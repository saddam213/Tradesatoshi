using System.Collections.Generic;

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

		public decimal Low { get; set; }
		public decimal High { get; set; }
		public decimal Last { get; set; }
	}
}