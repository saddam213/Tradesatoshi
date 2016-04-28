namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiCurrency
	{
		public string Currency { get; set; }
		public string CurrencyLong { get; set; }
		public int MinConfirmation { get; set; }
		public decimal TxFee { get; set; }
		public string Status { get; set; }
	}
}