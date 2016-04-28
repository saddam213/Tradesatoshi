using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiMarketSummary
	{
		public string Market { get; set; }
		public decimal High { get; set; }
		public decimal Low { get; set; }
		public decimal Volume { get; set; }
		public decimal BaseVolume
		{
			get
			{
				if (BaseVolumeList == null || !BaseVolumeList.Any())
					return 0.00000000m;

				return decimal.Round(BaseVolumeList.Sum(x => x.Amount * x.Rate), 8);
			}
		}
		public decimal Last { get; set; }
		public decimal Bid { get; set; }
		public decimal Ask { get; set; }
		public int OpenBuyOrders { get; set; }
		public int OpenSellOrders { get; set; }

		[IgnoreDataMember]
		public List<ApiVolume> BaseVolumeList { get; set; }
	}

	public class ApiVolume
	{
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
	}
}