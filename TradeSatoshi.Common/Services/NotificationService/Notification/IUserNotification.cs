namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface IUserNotification : INotification
	{
		string UserId { get; set; }
	}
}
