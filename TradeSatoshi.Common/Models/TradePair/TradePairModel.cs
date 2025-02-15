﻿using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.TradePair
{
	public class TradePairModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public TradePairStatus Status { get; set; }
		public int CurrencyId { get; set; }
		public int BaseCurrencyId { get; set; }
		public double Change { get; set; }
		public decimal LastTrade { get; set; }
		public string BaseSymbol{ get; set; }
		public string Symbol { get; set; }
		public int MarketSortOrder { get; set; }
	}
}