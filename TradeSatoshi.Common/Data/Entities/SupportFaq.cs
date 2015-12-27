using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data.Entities
{
	public class SupportFaq
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(256)]
		public string Question { get; set; }

		[MaxLength(4000)]
		public string Answer { get; set; }
	
		public int Order { get; set; }

		public bool IsEnabled { get; set; }
	}
}
