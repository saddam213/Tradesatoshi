using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface IDataNotification
	{
		string ElementName { get; set; }
		string ElementValue { get; set; }
	}
}
