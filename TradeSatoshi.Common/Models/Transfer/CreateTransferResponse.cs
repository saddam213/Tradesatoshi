namespace TradeSatoshi.Common.Trade
{
	public class CreateTransferResponse : ITradeResponse
	{
		public string Error { get; set; }

		public bool HasError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}
	}
}