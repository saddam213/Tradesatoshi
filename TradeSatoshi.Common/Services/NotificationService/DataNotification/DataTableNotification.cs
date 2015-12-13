using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class DataTableNotification : IDataTableNotification
	{
		public DataTableNotification() { }
		public DataTableNotification(string dataTableName)
		{
			DataTableName = dataTableName;
		}
		public string DataTableName { get; set; }
	}
}
