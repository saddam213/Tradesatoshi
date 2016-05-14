using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiTradeResponse
	{
		public int Id { get; set; }
		public string Market { get; set; }
		public string Type { get; set; }
		public decimal Amount { get; set; }
		public decimal Rate { get; set; }
		public decimal Fee { get; set; }
		public decimal Total
		{
			get { return Math.Round(Amount * Rate, 8) - Fee; }
		}
		public DateTime Timestamp { get; set; }
		public bool IsApi { get; set; }
	}
}