using System;
using System.Collections.Generic;
using TradeSatoshi.Common.Currency;

namespace TradeSatoshi.Common.Vote
{
	public class UpdateVoteSettingsModel
	{
		public List<CurrencyModel> Currencies { get; set; }
		public int CurrencyId { get; set; }
		public bool IsFreeEnabled { get; set; }
		public bool IsPaidEnabled { get; set; }
		public DateTime Next { get; set; }
		public decimal Price { get; set; }
	}
}
