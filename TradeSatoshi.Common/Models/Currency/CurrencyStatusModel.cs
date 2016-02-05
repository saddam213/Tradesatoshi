using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Currency
{
	public class CurrencyStatusModel
	{
		public int LastBlock { get; set; }

		public int CurrencyId { get; set; }
		public string Symbol { get; set; }

		public CurrencyStatus Status { get; set; }
		
		public string StatusMessage { get; set; }
		public int Connections { get; set; }
		public string Version { get; set; }
	}
}
