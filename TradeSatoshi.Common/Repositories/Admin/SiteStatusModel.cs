namespace TradeSatoshi.Common.Admin
{
	public class SiteStatusModel
	{
		public int Deposits { get; set; }
		public int Deposits24 { get; set; }
		public int Logons { get; set; }
		public int Logons24 { get; set; }
		public int Orders { get; set; }
		public int Orders24 { get; set; }
		public int Trades { get; set; }
		public int Trades24 { get; set; }
		public int Transfers { get; set; }
		public int Transfers24 { get; set; }
		public int Withdrawals { get; set; }
		public int Withdrawals24 { get; set; }
	}
}