using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TradeSatoshi.Common.Support
{
	public class UpdateSupportFaqModel
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(256)]
		public string Question { get; set; }

		[Required]
		[MaxLength(4000)]
		public string Answer { get; set; }

		[Required]
		public int Order { get; set; }

		public bool IsEnabled { get; set; }
	}
}
