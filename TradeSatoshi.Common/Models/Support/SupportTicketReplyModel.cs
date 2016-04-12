namespace TradeSatoshi.Common.Support
{
	public class SupportTicketReplyModel
	{
		public int Id { get; set; }
		public int TicketId { get; set; }
		public string UserName { get; set; }
		public string Message { get; set; }
		public bool IsAdmin { get; set; }
		public bool IsPublic { get; set; }
	}
}