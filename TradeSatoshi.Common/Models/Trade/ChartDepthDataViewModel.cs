using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
