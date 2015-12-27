using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TradeSatoshi.Common.Support
{
	public class CreateSupportTicketReplyModel
	{
		public int TicketId { get; set; }

		[Required]
		[MaxLength(4000)]
		public string Message { get; set; }

		public bool IsPublic { get; set; }
		public bool IsAdmin { get; set; }
	}
}
