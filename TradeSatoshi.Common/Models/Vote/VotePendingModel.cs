using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public class VotePendingModel
	{
		public string Name { get; set; }
		public VoteItemStatus Status { get; set; }
	}
}
