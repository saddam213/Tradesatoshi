using System;
using System.Runtime.Serialization;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiMarketHistory
	{
		public int Id { get; set; }
		public DateTime TimeStamp { get; set; }
		public decimal Quantity { get; set; }
		public decimal Price { get; set; }
			public string OrderType { get; set; }
		public decimal Total
		{
			get { return decimal.Round(Quantity * Price, 8); }
		}
	}
}