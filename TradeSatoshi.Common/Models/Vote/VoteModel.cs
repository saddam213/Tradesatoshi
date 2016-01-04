using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Vote
{
	public class VoteModel
	{
		public int VotePeriod { get; set; }
		public DateTime NextVote { get; set; }

		public int? LastFreeId { get; set; }

		public int? LastPaidId { get; set; }

		public string LastFree { get; set; }

		public string LastPaid { get; set; }
	}
}
