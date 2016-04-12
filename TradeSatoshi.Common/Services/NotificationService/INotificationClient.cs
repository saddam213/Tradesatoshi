using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface INotificationClient
	{
		Task OnNotification(Notification notification);
		Task OnUserNotification(UserNotification notification);
		Task OnNotifications(List<Notification> notifications);
		Task OnUserNotifications(List<UserNotification> notifications);
	}
}