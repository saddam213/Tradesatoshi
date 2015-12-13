using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface INotification
	{
		string Title { get; set; }
		string Message { get; set; }
		NotificationType Type { get; set; }
	}
}
