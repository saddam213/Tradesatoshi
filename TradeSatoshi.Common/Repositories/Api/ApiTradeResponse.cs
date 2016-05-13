using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiTradeResponse
	{
		public decimal Amount { get; set; }
		public decimal Fee { get; set; }
		public int Id { get; set; }
		public bool IsApi { get; set; }
		public string Market { get; set; }
		public decimal Rate { get; set; }
		public DateTime Timestamp { get; set; }
		public string Type { get; set; }
	}
}