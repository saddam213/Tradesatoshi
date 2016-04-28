namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiOrdersRequest
	{
		public string Market { get; set; } = "all";
		public int Count { get; set; } = 20;
	}
}