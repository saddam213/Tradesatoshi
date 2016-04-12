namespace TradeSatoshi.Common.Services.NotificationService
{
	public class DataTableNotification : IDataTableNotification
	{
		public DataTableNotification()
		{
		}

		public DataTableNotification(string dataTableName)
		{
			DataTableName = dataTableName;
		}

		public string DataTableName { get; set; }
	}
}