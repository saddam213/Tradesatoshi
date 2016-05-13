using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.NotificationService;

namespace TradeSatoshi.Web.Hubs
{
	[HubName("Notification")]
	public class NotificationHub : Hub
	{
		public async Task OnNotification(NotifyUser notification)
		{
			await Clients.User(notification.UserId).OnNotification(notification);
		}

		public async Task OnBalanceUpdate(NotifyBalanceUpdate notification)
		{
			await Clients.User(notification.UserId).OnBalanceUpdate(notification);
		}

		public async Task OnOrderBookUpdate(NotifyOrderBookUpdate notification)
		{
			await Clients.All.OnOrderBookUpdate(notification);
		}

		public async Task OnTradeHistoryUpdate(NotifyTradeHistoryUpdate notification)
		{
			await Clients.All.OnTradeHistoryUpdate(notification);
		}

		public async Task OnOpenOrderUserUpdate(NotifyOpenOrderUserUpdate notification)
		{
			await Clients.User(notification.UserId).OnOpenOrderUserUpdate(notification);
		}

		public async Task OnTradeUserHistoryUpdate(NotifyTradeUserHistoryUpdate notification)
		{
			await Clients.User(notification.UserId).OnTradeUserHistoryUpdate(notification);
		}
	}
}

