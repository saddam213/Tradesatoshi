using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public class VoteItemModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int VoteCount { get; set; }
		public VoteType VoteType { get; set; }
	}
}