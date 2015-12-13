using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class UserDataTableNotification : IUserDataTableNotification
	{
		public UserDataTableNotification() { }
		public UserDataTableNotification(string userId, string dataTableName)
		{
			UserId = userId;
			DataTableName = dataTableName;
		}
		public string UserId { get; set; }
		public string DataTableName { get; set; }
	}
}
