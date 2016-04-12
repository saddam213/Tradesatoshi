namespace TradeSatoshi.Common.Services.NotificationService
{
	public interface IDataNotification
	{
		string DataKey { get; set; }
		object DataValue { get; set; }
		bool IsElementKey { get; set; }
	}
}