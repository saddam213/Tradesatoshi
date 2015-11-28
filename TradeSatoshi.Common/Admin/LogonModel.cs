using Mvc.JQuery.Datatables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Admin
{

	public class LogonModel
	{
		public string UserName { get; set; }
		public string IPAddress { get; set; }
		public string IsValid { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
