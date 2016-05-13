using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface INotificationService
	{
		Task<bool> SendNotification(NotifyUser notification);
		Task<bool> SendNotification(List<NotifyUser> notifications);
		Task<bool> SendBalanceUpdate(NotifyBalanceUpdate notification);
		Task<bool> SendBalanceUpdate(List<NotifyBalanceUpdate> notifications);
		Task<bool> SendOrderBookUpdate(NotifyOrderBookUpdate notification);
		Task<bool> SendOrderBookUpdate(List<NotifyOrderBookUpdate> notifications);
		Task<bool> SendTradeHistoryUpdate(NotifyTradeHistoryUpdate notification);
		Task<bool> SendTradeHistoryUpdate(List<NotifyTradeHistoryUpdate> notifications);
		Task<bool> SendOpenOrderUserUpdate(NotifyOpenOrderUserUpdate notification);
		Task<bool> SendOpenOrderUserUpdate(List<NotifyOpenOrderUserUpdate> notifications);
		Task<bool> SendTradeUserHistoryUpdate(NotifyTradeUserHistoryUpdate notification);
		Task<bool> SendTradeUserHistoryUpdate(List<NotifyTradeUserHistoryUpdate> notifications);
		Task<bool> SendNotificationCollection(List<INotify> notifications);
	}
}
