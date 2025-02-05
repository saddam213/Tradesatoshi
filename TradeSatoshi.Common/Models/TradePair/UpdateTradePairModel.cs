﻿using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.TradePair
{
	public class UpdateTradePairModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public TradePairStatus Status { get; set; }
		public string StatusMessage { get; set; }
	}
}