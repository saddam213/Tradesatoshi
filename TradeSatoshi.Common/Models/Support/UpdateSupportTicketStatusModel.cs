using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Support
{
	public class UpdateSupportTicketStatusModel
	{
		public int TicketId { get; set; }
		public SupportTicketStatus Status { get; set; }
	}
}