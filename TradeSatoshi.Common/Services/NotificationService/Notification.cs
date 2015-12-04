using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class Notification
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public NotificationType Type { get; set; }
	}
}
