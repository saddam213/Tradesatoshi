using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Services.TradeNotificationService
{
	public interface ITradeNotificationService
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

	public interface INotify
	{

	}

	public class NotifyUser : INotify
	{
		public string UserId { get; set; }
		public NotificationType Type { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
	}

	public class NotifyBalanceUpdate : INotify
	{
		public string UserId { get; set; }
		public string Symbol { get; set; }
		public decimal Total { get; set; }
		public decimal Unconfirmed { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }

		public decimal Avaliable { get; set; }

		public int CurrencyId { get; set; }
	}

	public class NotifyOrderBookUpdate : INotify
	{
		public string Market { get; set; }
		public string Type { get; set; }
		public string Action { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public object TradePairId { get; set; }
	}

	public class NotifyTradeHistoryUpdate : INotify
	{
		public string Market { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public string Type { get; set; }
		public DateTime Timestamp { get; set; }
		public int TradePairId { get; set; }
	}

	public class NotifyTradeUserHistoryUpdate : INotify
	{
		public string UserId { get; set; }
		public string Market { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public string Type { get; set; }
		public DateTime Timestamp { get; set; }
		public int TradePairId { get; set; }
	}

	public class NotifyOpenOrderUserUpdate : INotify
	{
		public string UserId { get; set; }
		public string Market { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public decimal Remaining { get; set; }
		public string Type { get; set; }
		public DateTime Timestamp { get; set; }
		public string Action { get; set; }
		public int OrderId { get; set; }
		public object TradePairId { get; set; }
	}

}
