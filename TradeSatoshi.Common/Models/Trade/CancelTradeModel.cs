using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Trade
{
	public class CancelTradeModel : ITradeItem
	{
		public string UserId { get; set; }
		public int? OrderId { get; set; }
		public string Market { get; set; }
		public TradeCancelType CancelType { get; set; }
		public bool IsApi { get; set; }
	}
}