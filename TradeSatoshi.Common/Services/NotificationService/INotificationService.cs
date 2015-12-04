using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface INotificationService
	{
		bool SendNotification(Notification notification);
		bool SendUserNotification(string userId, Notification notification);

		Task<bool> SendNotificationAsync(Notification notification);
		Task<bool> SendUserNotificationAsync(string userId, Notification notification);
	}
}
