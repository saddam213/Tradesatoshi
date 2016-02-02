using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Currency
{
	public class CurrencyModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public CurrencyStatus Status { get; set; }
		public string Version { get; set; }
		public decimal Balance { get; set; }
		public int Connections { get; set; }
		public int Block { get; set; }
		public string Error { get; set; }
		public bool IsEnabled { get; set; }
	}
}
