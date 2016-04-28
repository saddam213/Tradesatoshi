using System;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiWithdrawResponse
	{
		public string Address { get; set; }
		public decimal Amount { get; set; }
		public int Confirmations { get; set; }
		public string Currency { get; set; }
		public string CurrencyLong { get; set; }
		public decimal Fee { get; set; }
		public int Id { get; set; }
		public string Status { get; set; }
		public DateTime TimeStamp { get; set; }
		public string Txid { get; set; }
	}
}