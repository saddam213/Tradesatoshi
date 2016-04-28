namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiSubmitWithdrawRequest
	{
		public string Currency { get; set; }
		public string Address { get; set; }
		public decimal Amount { get; set; }
	}
}