using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Entity
{
	public class SupportCategory
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }

		public bool IsEnabled { get; set; }

		public virtual ICollection<SupportTicket> SupportTickets { get; set; }
	}
}
