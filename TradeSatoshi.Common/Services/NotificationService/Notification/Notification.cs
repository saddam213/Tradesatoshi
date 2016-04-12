using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class Notification : INotification
	{
		public Notification()
		{
		}

		public Notification(NotificationType type, string title, string message, params object[] messageFormatParams)
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