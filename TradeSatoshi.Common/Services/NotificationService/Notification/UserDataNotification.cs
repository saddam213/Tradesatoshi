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
		public UserDataNotification(string userId, string elementName, string elementValue)
		{
			UserId = userId;
			ElementName = elementName;
			ElementValue = elementValue;
		}
		public string UserId { get; set; }
		public string ElementName { get; set; }
		public string ElementValue { get; set; }
	}
}
