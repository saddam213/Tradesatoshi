using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class NotifyUser : INotify
	{
		public string UserId { get; set; }
		public NotificationType Type { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
	}
}
