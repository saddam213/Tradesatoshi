using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Trade;

namespace TradeSatoshi.Common.Faucet
{
	public class CreateFaucetPaymentModel : ITradeItem
	{
		public int CurrencyId { get; set; }
		public string IPAddress { get; set; }

		#region ITradeItem

		public string UserId { get; set; }
		public bool IsApi { get; set; }

		#endregion
	}
}
