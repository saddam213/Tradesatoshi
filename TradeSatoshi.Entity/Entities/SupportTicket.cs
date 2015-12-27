using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Entity
{
	public class SupportTicket
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[MaxLength(256)]
		public string Title { get; set; }

		[MaxLength(4000)]
		public string Description { get; set; }

		public int CategoryId { get; set; }

		public SupportTicketStatus Status { get; set; }

		public DateTime LastUpdate { get; set; }

		public DateTime Created { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("CategoryId")]
		public virtual SupportCategory Category { get; set; }

		public virtual ICollection<SupportTicketReply> Replies { get; set; }
	}
}
