namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiBalanceResponse
	{
		public string Currency { get; set; }
		public string CurrencyLong { get; set; }
		public decimal Avaliable
		{
			get { return Total - (Unconfirmed + HeldForTrades + PendingWithdraw); }
		}
		public decimal Total { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal Unconfirmed { get; set; }
		public decimal PendingWithdraw { get; set; }
		public string Address { get; set; }
	}
}