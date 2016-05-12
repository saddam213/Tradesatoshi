using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiCancelOrderRequest
	{
		public TradeCancelType Type { get; set; }
		public int? OrderId { get; set; }
		public string Market { get; set; }
	}
}