namespace TradeSatoshi.Common.Trade
{
	public interface ITradeResponse
	{
		bool HasError { get; }
		string Error { get; set; }
	}
}