using System;
using System.Collections.Generic;

namespace TradeSatoshi.Common.Vote
{
	public class VoteModel : ITradeSidebarModel
	{
		public int VotePeriod { get; set; }
		public DateTime NextVote { get; set; }

		public int? LastFreeId { get; set; }

		public int? LastPaidId { get; set; }

		public string LastFree { get; set; }

		public string LastPaid { get; set; }

		public List<Balance.BalanceModel> Balances { get; set; }

		public List<TradePair.TradePairModel> TradePairs { get; set; }
	}
}
