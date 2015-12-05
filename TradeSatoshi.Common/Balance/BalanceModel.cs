using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Balance
{
	public class BalanceModel
	{
		public string Currency { get; set; }
		public string Symbol { get; set; }
		public decimal Total { get; set; }
		public decimal Avaliable
		{
			get { return Total - (Unconfirmed + HeldForTrades + PendingWithdraw); }
		}
		public decimal Unconfirmed { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }
	}
}
