using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.TradeNotificationService;

namespace TradeSatoshi.Core.Services
{

	public class TradeNotificationService : ITradeNotificationService
	{
		private readonly string _notificationProxyName = "TradeNotification";
		private readonly string _connectionUrl = ConfigurationManager.AppSettings["ClientNotificationUrl"];


		public async Task<bool> SendOrderBookUpdate(NotifyOrderBookUpdate notification)
		{
			return await SendOrderBookUpdate(new List<NotifyOrderBookUpdate> { notification });
		}

		public async Task<bool> SendOrderBookUpdate(List<NotifyOrderBookUpdate> notifications)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					var proxy = connection.CreateHubProxy(_notificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					foreach (var notification in notifications)
					{
						await proxy.Invoke("OnOrderBookUpdate", notification);
					}
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}



		public async Task<bool> SendTradeHistoryUpdate(NotifyTradeHistoryUpdate notification)
		{
			return await SendTradeHistoryUpdate(new List<NotifyTradeHistoryUpdate> { notification });
		}

		public async Task<bool> SendTradeHistoryUpdate(List<NotifyTradeHistoryUpdate> notifications)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					var proxy = connection.CreateHubProxy(_notificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					foreach (var notification in notifications)
					{
						await proxy.Invoke("OnTradeHistoryUpdate", notification);
					}
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}


		public async Task<bool> SendTradeUserHistoryUpdate(NotifyTradeUserHistoryUpdate notification)
		{
			return await SendTradeUserHistoryUpdate(new List<NotifyTradeUserHistoryUpdate> { notification });
		}

		public async Task<bool> SendTradeUserHistoryUpdate(List<NotifyTradeUserHistoryUpdate> notifications)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					var proxy = connection.CreateHubProxy(_notificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					foreach (var notification in notifications)
					{
						await proxy.Invoke("OnTradeUserHistoryUpdate", notification);
					}
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendNotification(NotifyUser notification)
		{
			return await SendNotification(new List<NotifyUser> { notification });
		}

		public async Task<bool> SendNotification(List<NotifyUser> notifications)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					var proxy = connection.CreateHubProxy(_notificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					foreach (var notification in notifications)
					{
						await proxy.Invoke("OnNotification", notification);
					}
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendBalanceUpdate(NotifyBalanceUpdate notification)
		{
			return await SendBalanceUpdate(new List<NotifyBalanceUpdate> { notification });
		}

		public async Task<bool> SendBalanceUpdate(List<NotifyBalanceUpdate> notifications)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					var proxy = connection.CreateHubProxy(_notificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					foreach (var notification in notifications)
					{
						await proxy.Invoke("OnBalanceUpdate", notification);
					}
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendOpenOrderUserUpdate(NotifyOpenOrderUserUpdate notification)
		{
			return await SendOpenOrderUserUpdate(new List<NotifyOpenOrderUserUpdate> { notification });
		}

		public async Task<bool> SendOpenOrderUserUpdate(List<NotifyOpenOrderUserUpdate> notifications)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					var proxy = connection.CreateHubProxy(_notificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					foreach (var notification in notifications)
					{
						await proxy.Invoke("OnOpenOrderUserUpdate", notification);
					}
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}



		public async Task<bool> SendNotificationCollection(List<INotify> notifications)
		{
			try
			{
				using (var connection = new HubConnection(_connectionUrl))
				{
					ServicePointManager.DefaultConnectionLimit = 100000;
					var proxy = connection.CreateHubProxy(_notificationProxyName);
					if (proxy == null)
						return false;

					await connection.Start();
					foreach (var notification in notifications)
					{
						if (notification is NotifyOrderBookUpdate)
							await proxy.Invoke("OnOrderBookUpdate", notification);
						else if (notification is NotifyTradeHistoryUpdate)
							await proxy.Invoke("OnTradeHistoryUpdate", notification);
						else if (notification is NotifyTradeUserHistoryUpdate)
							await proxy.Invoke("OnTradeUserHistoryUpdate", notification);
						else if (notification is NotifyUser)
							await proxy.Invoke("OnNotification", notification);
						else if (notification is NotifyBalanceUpdate)
							await proxy.Invoke("OnBalanceUpdate", notification);
						else if (notification is NotifyOpenOrderUserUpdate)
							await proxy.Invoke("OnOpenOrderUserUpdate", notification);
					}
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}