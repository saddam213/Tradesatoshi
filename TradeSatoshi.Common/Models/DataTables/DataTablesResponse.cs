using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.DataTables
{
	public class DataTablesResponse
	{
		public int iTotalRecords { get; set; }
		public int iTotalDisplayRecords { get; set; }
		public int sEcho { get; set; }
		public object[] aaData { get; set; }
	}
}
