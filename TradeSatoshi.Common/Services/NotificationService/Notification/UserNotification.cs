using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class UserNotification : IUserNotification
	{
		public UserNotification() { }
		public UserNotification(NotificationType type, string userId, string title, string message, params object[] messageFormatParams)
		{
			Type = type;
			Title = title;
			UserId = userId;
			Message = string.Format(message, messageFormatParams);
		}

		public string UserId { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public NotificationType Type { get; set; }
	}
}
