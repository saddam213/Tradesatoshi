using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class NotifyOpenOrderUserUpdate : INotify
	{
		public string UserId { get; set; }
		public string Market { get; set; }
		public decimal Price { get; set; }
		public decimal Amount { get; set; }
		public decimal Remaining { get; set; }
		public string Type { get; set; }
		public DateTime Timestamp { get; set; }
		public string Action { get; set; }
		public int OrderId { get; set; }
		public object TradePairId { get; set; }
	}
}
