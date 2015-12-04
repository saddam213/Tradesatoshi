using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface INotificationClient
	{
		Task OnNotification(Notification notification);
		Task OnUserNotification(string userId, Notification notification);
	}
}
