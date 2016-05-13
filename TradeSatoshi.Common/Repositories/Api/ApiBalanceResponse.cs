namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiBalanceResponse
	{
		public string Address { get; set; }
		public string Currency { get; set; }
		public string CurrencyLong { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }
		public decimal Total { get; set; }
		public decimal Unconfirmed { get; set; }
	}
}