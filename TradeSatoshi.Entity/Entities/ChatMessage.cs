using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Entity
{
	public class ChatMessage
	{
		[Key]
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Message { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsEnabled { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}
