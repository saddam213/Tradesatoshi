using System;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public class AdminVoteItemModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public VoteItemStatus Status { get; set; }
		public string CreatedBy { get; set; }
		public DateTime Created { get; set; }
		public int VoteCountFree { get; set; }
		public int VoteCountPaid { get; set; }
	}
}