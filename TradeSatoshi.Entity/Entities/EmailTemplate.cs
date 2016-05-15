using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Entity
{
	public class EmailTemplate
	{
		[Key]
		public int Id { get; set; }

		public EmailType Type { get; set; }

		[MaxLength(256)]
		public string From { get; set; }

		[MaxLength(256)]
		public string Subject { get; set; }

		[MaxLength(4000)]
		public string Template { get; set; }

		[MaxLength(1000)]
		public string Description { get; set; }

		[MaxLength(1000)]
		public string Parameters { get; set; }

		public bool IsHtml { get; set; }

		public bool IsEnabled { get; set; }
	}
}
