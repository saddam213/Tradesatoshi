using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class DataNotification : IDataNotification
	{
		public DataNotification() { }
		public DataNotification(string elementName, string elementValue)
		{
			ElementName = elementName;
			ElementValue = elementValue;
		}
		public string ElementName { get; set; }
		public string ElementValue { get; set; }
	}
}
