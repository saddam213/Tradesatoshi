using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface INotification
	{
		string Title { get; set; }
		string Message { get; set; }
		NotificationType Type { get; set; }
	}
}