using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.NotificationService;

namespace TradeSatoshi.Core.Services
{
	public class NotificationService : INotificationService
	{
		private readonly string ProxyName = "Notification";
		private readonly string ConnectionUrl = ConfigurationManager.AppSettings["ClientNotificationUrl"];

		public bool SendNotification(Notification notification)
		{
			using (var connection = new HubConnection(ConnectionUrl))
			{
				var proxy = connection.CreateHubProxy(ProxyName);
				if (proxy == null)
					return false;

				connection.Start().Wait();
				proxy.Invoke("OnNotification", notification);
				return true;
			}
		}

		public async Task<bool> SendNotificationAsync(Notification notification)
		{
			using (var connection = new HubConnection(ConnectionUrl))
			{
				var proxy = connection.CreateHubProxy(ProxyName);
				if (proxy == null)
					return false;

				await connection.Start();
				await proxy.Invoke("OnNotification", notification);
				return true;
			}
		}

		public bool SendUserNotification(string userId, Notification notification)
		{
			using (var connection = new HubConnection(ConnectionUrl))
			{
				var proxy = connection.CreateHubProxy(ProxyName);
				if (proxy == null)
					return false;

				connection.Start().Wait();
				proxy.Invoke("OnUserNotification", userId, notification);
				return true;
			}
		}

		public async Task<bool> SendUserNotificationAsync(string userId, Notification notification)
		{
			using (var connection = new HubConnection(ConnectionUrl))
			{
				var proxy = connection.CreateHubProxy(ProxyName);
				if (proxy == null)
					return false;

				await connection.Start();
				await proxy.Invoke("OnUserNotification", userId, notification);
				return true;
			}
		}
	}
}
