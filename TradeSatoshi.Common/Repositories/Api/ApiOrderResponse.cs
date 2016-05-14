using System;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiOrderResponse
	{
		public int Id { get; set; }
		public string Market { get; set; }
		public string Type { get; set; }
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public decimal Remaining { get; set; }
		public decimal Total 
		{
		 get { return Math.Round(Amount * Rate, 8); } 
		}
		public string Status { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsApi { get; set; }
	}
}