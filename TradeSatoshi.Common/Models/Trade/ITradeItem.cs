namespace TradeSatoshi.Common.Trade
{
	public interface ITradeItem
	{
		string UserId { get; set; }
		bool IsApi { get; set; }
	}
}