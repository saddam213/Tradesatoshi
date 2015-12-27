using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeSatoshi.Common;

namespace TradeSatoshi.Common.Support
{
	public class UpdateSupportReplyStatusModel
	{
		public int TicketId { get; set; }
		public int ReplyId { get; set; }
		public bool IsPublic { get; set; }
	}
}
