using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.NotificationService
{
	public class NotifyBalanceUpdate : INotify
	{
		public string UserId { get; set; }
		public string Symbol { get; set; }
		public decimal Total { get; set; }
		public decimal Unconfirmed { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }

		public decimal Avaliable { get; set; }

		public int CurrencyId { get; set; }
	}
}
