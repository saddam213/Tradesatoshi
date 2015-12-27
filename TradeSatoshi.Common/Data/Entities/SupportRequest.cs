using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data.Entities
{
	public class SupportRequest
	{
		[Key]
		public int Id { get; set; }

		[EmailAddress]
		public string Sender { get; set; }

		[MaxLength(256)]
		public string Title { get; set; }

		[MaxLength(4000)]
		public string Description { get; set; }

		public DateTime Created { get; set; }

		public bool Replied { get; set; }
	}
}
