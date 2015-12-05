using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Balance
{
	public class CurrencyModel
	{
		public string Name { get; set; }
		public string Symbol { get; set; }
		public CurrencyStatus Status { get; set; }
	}
}
