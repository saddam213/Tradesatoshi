using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface INotificationService
	{
		Task<bool> SendNotificationAsync(INotification notification);
		Task<bool> SendNotificationAsync(List<INotification> notifications);
		Task<bool> SendUserNotificationAsync(IUserNotification notification);
		Task<bool> SendUserNotificationAsync(List<IUserNotification> notifications);

		Task<bool> SendDataNotificationAsync(IDataNotification notification);
		Task<bool> SendDataNotificationAsync(List<IDataNotification> notifications);
		Task<bool> SendUserNotificationDataAsync(IUserDataNotification notification);
		Task<bool> SendUserNotificationDataAsync(List<IUserDataNotification> notifications);

		Task<bool> SendDataTableNotificationAsync(IDataTableNotification notification);
		Task<bool> SendDataTableNotificationAsync(List<IDataTableNotification> notifications);
		Task<bool> SendUserDataTableNotificationAsync(IUserDataTableNotification notification);
		Task<bool> SendUserDataTableNotificationAsync(List<IUserDataTableNotification> notifications);
	}
}