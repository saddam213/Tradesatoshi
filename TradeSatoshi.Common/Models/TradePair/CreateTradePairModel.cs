using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.TradePair
{
	public class CreateTradePairModel
	{
		public int CurrencyId2 { get; set; }
		public int CurrencyId1 { get; set; }
		public TradePairStatus Status { get; set; }
		public List<CurrencyModel> Currencies { get; set; }
	}
}
