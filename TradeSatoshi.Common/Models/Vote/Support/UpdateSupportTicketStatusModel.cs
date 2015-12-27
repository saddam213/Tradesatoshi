using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeSatoshi.Common;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Support
{
	public class UpdateSupportTicketStatusModel
	{
		public int TicketId { get; set; }
		public SupportTicketStatus Status { get; set; }
	}
}
