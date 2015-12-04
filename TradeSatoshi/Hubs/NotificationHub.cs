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

		public async Task OnUserNotification(string userId, Notification notification)
		{
			await Clients.User(userId).SendNotification(notification);
		}
	}
}