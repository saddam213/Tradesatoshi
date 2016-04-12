namespace TradeSatoshi.Common.Support
{
	public class UpdateSupportReplyStatusModel
	{
		public int TicketId { get; set; }
		public int ReplyId { get; set; }
		public bool IsPublic { get; set; }
	}
}