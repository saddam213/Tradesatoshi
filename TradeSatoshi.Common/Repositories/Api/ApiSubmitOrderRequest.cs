using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiSubmitOrderRequest
	{
		public string Market { get; set; }
		public TradeType Type { get; set; }
		public decimal Amount { get; set; }
		public decimal Price { get; set; }
	}
}