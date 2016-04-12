using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface IDataNotificationClient
	{
		Task OnDataNotification(DataNotification notification);
		Task OnDataNotifications(List<DataNotification> notifications);

		Task OnUserDataNotification(UserDataNotification notification);
		Task OnUserDataNotifications(List<UserDataNotification> notifications);

		Task OnDataTableNotification(DataTableNotification notification);
		Task OnDataTableNotifications(List<DataTableNotification> notifications);

		Task OnUserDataTableNotification(UserDataTableNotification notification);
		Task OnUserDataTableNotifications(List<UserDataTableNotification> notifications);
	}
}