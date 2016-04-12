using System.Collections.Generic;

namespace TradeSatoshi.Common.Support
{
	public class SupportUserModel
	{
		public SupportUserModel()
		{
			Tickets = new List<SupportTicketModel>();
			SupportFaq = new List<SupportFaqModel>();
		}

		public List<SupportFaqModel> SupportFaq { get; set; }
		public List<SupportTicketModel> Tickets { get; set; }
	}
}