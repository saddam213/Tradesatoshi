namespace TradeSatoshi.Common.Services.NotificationService
{
	public class DataNotification : IDataNotification
	{
		public DataNotification()
		{
		}

		public DataNotification(string dataKey, object dataValue)
		{
			DataKey = dataKey;
			DataValue = dataValue;
			IsElementKey = dataKey.StartsWith("#") || dataKey.StartsWith(".");
		}

		public string DataKey { get; set; }
		public object DataValue { get; set; }
		public bool IsElementKey { get; set; }
	}
}