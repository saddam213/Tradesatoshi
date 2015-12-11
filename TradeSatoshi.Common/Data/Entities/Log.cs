using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data.Entities
{
	public class Log
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string Type { get; set; }

		[MaxLength(128)]
		public string Component { get; set; }

		[MaxLength(4000)]
		public string Message { get; set; }

		public DateTime Timestamp { get; set; }
	}
}
