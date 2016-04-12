namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface IUserDataTableNotification : IDataTableNotification
	{
		string UserId { get; set; }
	}
}