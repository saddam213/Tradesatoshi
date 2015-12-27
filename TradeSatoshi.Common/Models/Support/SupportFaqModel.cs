using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Support
{
	public class SupportFaqModel
	{
		public int Id { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public int Order { get; set; }

		public bool IsEnabled { get; set; }
	}
}
