using System.Collections.Generic;

namespace TradeSatoshi.Common.Admin
{
	public class SiteStatusModel
	{
		public List<SiteProfitModel> Profit { get; set; }
		public List<SiteStatisticModel> Statistic { get; set; }
	}

	public class SiteStatisticModel
	{
		public int AllTime { get; set; }
		public int Today { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }
		public string Name { get; set; }
	}

	public class SiteProfitModel
	{
		public string Name { get; set; }
		public decimal UserBalance { get; set; }
		public decimal Balance { get; set; }
		public decimal ColdBalance { get; set; }
		public decimal Liquid
		{
			get { return (Balance + ColdBalance) - UserBalance; }
		}

		public decimal TotalDeposit { get; set; }
		public decimal TotalWithdraw { get; set; }
	}
}