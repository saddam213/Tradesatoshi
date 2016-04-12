using System.Collections.Generic;

namespace TradeSatoshi.Common.Trade
{
	public class ChartDepthDataViewModel
	{
		public ChartDepthDataViewModel()
		{
			SellDepth = new List<decimal[]>();
			BuyDepth = new List<decimal[]>();
		}

		public List<decimal[]> SellDepth { get; set; }
		public List<decimal[]> BuyDepth { get; set; }
	}
}