using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TradeSatoshi.Common.Services.NotificationService;

namespace TradeSatoshi.Hubs
{
	[HubName("Notification")]
	public class NotificationHub : Hub, INotificationClient
	{
		public async Task OnNotification(Notification notification)
		{
			await Clients.All.SendNotification(notification);
		}

		public async Task OnUserNotification(UserNotification notification)
		{
			await Clients.User(notification.UserId).SendNotification(notification);
		}

		public async Task OnNotifications(List<Notification> notifications)
		{
			foreach (var notification in notifications)
			{
				await Clients.All.SendNotification(notification);
			}
		}

		public async Task OnUserNotifications(List<UserNotification> notifications)
		{
			foreach (var notification in notifications)
			{
				await Clients.User(notification.UserId).SendNotification(notification);
			}
		}
	}
}