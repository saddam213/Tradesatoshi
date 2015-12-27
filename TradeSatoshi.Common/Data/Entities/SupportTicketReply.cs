using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data.Entities
{
	public class SupportTicketReply
	{
		[Key]
		public int Id { get; set; }

		public int TicketId { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[MaxLength(4000)]
		public string Message { get; set; }

		public bool IsPublic { get; set; }

		public bool IsAdmin { get; set; }

		public DateTime Created { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("TicketId")]
		public virtual SupportTicket Ticket { get; set; }
	}
}
