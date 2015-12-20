using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.NotificationService;

namespace TradeSatoshi.Web.Hubs
{
	[HubName("DataNotification")]
	public class DataNotificationHub : Hub, IDataNotificationClient
	{
		public async Task OnDataNotification(DataNotification notification)
		{
			await Clients.All.UpdateData(notification);
		}

		public async Task OnUserDataNotification(UserDataNotification notification)
		{
			await Clients.User(notification.UserId).UpdateData(notification);
		}

		public async Task OnDataNotifications(List<DataNotification> notifications)
		{
			foreach (var notification in notifications)
			{
				await Clients.All.UpdateData(notification);
			}
		}

		public async Task OnUserDataNotifications(List<UserDataNotification> notifications)
		{
			foreach (var notification in notifications)
			{
				await Clients.User(notification.UserId).UpdateData(notification);
			}
		}

		public async Task OnDataTableNotification(DataTableNotification notification)
		{
			await Clients.All.UpdateDataTable(notification);
		}

		public async Task OnDataTableNotifications(List<DataTableNotification> notifications)
		{
			foreach (var notification in notifications)
			{
				await Clients.All.UpdateDataTable(notification);
			}
		}

		public async Task OnUserDataTableNotification(UserDataTableNotification notification)
		{
			await Clients.User(notification.UserId).UpdateDataTable(notification);
		}

		public async Task OnUserDataTableNotifications(List<UserDataTableNotification> notifications)
		{
			foreach (var notification in notifications)
			{
				await Clients.User(notification.UserId).UpdateDataTable(notification);
			}
		}
	}
}