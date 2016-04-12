namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface IUserDataNotification : IDataNotification
	{
		string UserId { get; set; }
	}
}