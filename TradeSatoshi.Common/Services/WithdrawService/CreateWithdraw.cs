using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.WalletService
{
	public class CreateWithdraw
	{
		public string Address { get; set; }
		public decimal Amount { get; set; }
		public string ConfirmationToken { get; set; }
		public int CurrencyId { get; set; }
		public bool IsApi { get; set; }
		public string UserId { get; set; }
	}
}
