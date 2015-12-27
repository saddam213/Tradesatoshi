using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class Notification : INotification
	{
		public Notification() { }
		public Notification(NotificationType type, string userId, string title, string message, params object[] messageFormatParams)
		{
			Type = type;
			Title = title;
			Message = string.Format(message, messageFormatParams);
		}

		public string Title { get; set; }
		public string Message { get; set; }
		public NotificationType Type { get; set; }
	}
}
