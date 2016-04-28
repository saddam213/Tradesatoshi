namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiCancelOrdersRequest
	{
		public string Market { get; set; }
		public string Type { get; set; } = "all";
	}
}