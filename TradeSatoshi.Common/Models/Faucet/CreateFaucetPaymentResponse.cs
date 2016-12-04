using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Trade;

namespace TradeSatoshi.Common.Faucet
{
	public class CreateFaucetPaymentResponse : ITradeResponse
	{
		public string Error { get; set; }

		public bool HasError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}
	}

}
