using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Support
{
	public class SupportTicketModel
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public SupportTicketStatus Status { get; set; }
		public List<SupportTicketReplyModel> Replies { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdate { get; set; }
	}
}
