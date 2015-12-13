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
		private readonly string NotificationProxyName = "Notification";
		private readonly string DataNotificationProxyName = "DataNotification";
		private readonly string ConnectionUrl = ConfigurationManager.AppSettings["ClientNotificationUrl"];

		#region Notification

		public async Task<bool> SendNotificationAsync(INotification notification)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(NotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnNotification", notification);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendNotificationAsync(List<INotification> notifications)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(NotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnNotifications", notifications);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool SendNotification(INotification notification)
		{
			return Task.Run(() => SendNotificationAsync(notification)).Result;
		}
		public bool SendNotification(List<INotification> notifications)
		{
			return Task.Run(() => SendNotificationAsync(notifications)).Result;
		}

		#endregion

		#region UserNotification

		public async Task<bool> SendUserNotificationAsync(IUserNotification notification)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(NotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnUserNotification", notification);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendUserNotificationAsync(List<IUserNotification> notifications)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(NotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnUserNotifications", notifications);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool SendUserNotification(IUserNotification notification)
		{
			return Task.Run(() => SendUserNotificationAsync(notification)).Result;
		}

		public bool SendUserNotification(List<IUserNotification> notifications)
		{
			return Task.Run(() => SendUserNotificationAsync(notifications)).Result;
		}

		#endregion

		#region DataUpdate

		public async Task<bool> SendDataNotificationAsync(IDataNotification notification)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(DataNotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnDataNotification", notification);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendUserNotificationDataAsync(IUserDataNotification notification)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(DataNotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnUserDataNotification", notification);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendDataNotificationAsync(List<IDataNotification> notifications)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(DataNotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnDataNotifications", notifications);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendUserNotificationDataAsync(List<IUserDataNotification> notifications)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(DataNotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnUserDataNotifications", notifications);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		#endregion

		#region DataTables

		public async Task<bool> SendDataTableNotificationAsync(IDataTableNotification notification)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(DataNotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnDataTableNotification", notification);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendDataTableNotificationAsync(List<IDataTableNotification> notifications)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(DataNotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnDataTableNotifications", notifications);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendUserDataTableNotificationAsync(IUserDataTableNotification notification)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(DataNotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnUserDataTableNotification", notification);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendUserDataTableNotificationAsync(List<IUserDataTableNotification> notifications)
		{
			try
			{
				using (var connection = new HubConnection(ConnectionUrl))
				{
					var proxy = connection.CreateHubProxy(DataNotificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					await proxy.Invoke("OnUserDataTableNotifications", notifications);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		#endregion
	}
}
