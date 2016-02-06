using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface IDataNotification
	{
		string DataKey { get; set; }
		object DataValue { get; set; }
		bool IsElementKey { get; set; }
	}
}
