using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class UserDataNotification : IUserDataNotification
	{
		public UserDataNotification() { }
		public UserDataNotification(string userId, string dataKey, string dataValue)
		{
			UserId = userId;
			DataKey = dataKey;
			DataValue = dataValue;
			IsElementKey = dataKey.StartsWith("#") || dataKey.StartsWith(".");
		}
		public string UserId { get; set; }
	
		public string DataKey { get; set; }

		public object DataValue { get; set; }

		public bool IsElementKey { get; set; }
	}
}
