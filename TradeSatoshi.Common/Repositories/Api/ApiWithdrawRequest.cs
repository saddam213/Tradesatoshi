namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiWithdrawRequest
	{
		public string Currency { get; set; } = "all";
		public int Count { get; set; } = 20;
	}
}