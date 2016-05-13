namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiTradeRequest
	{
		public string Market { get; set; } = "all";
		public int Count { get; set; } = 20;
	}
}