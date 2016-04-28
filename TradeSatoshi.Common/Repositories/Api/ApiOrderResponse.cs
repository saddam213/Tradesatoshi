using System;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiOrderResponse
	{
		public decimal Amount { get; set; }
		public decimal Fee { get; set; }
		public int Id { get; set; }
		public bool IsApi { get; set; }
		public string Market { get; set; }
		public decimal Rate { get; set; }
		public decimal Remaining { get; set; }
		public string Status { get; set; }
		public DateTime Timestamp { get; set; }
		public string Type { get; set; }
	}
}