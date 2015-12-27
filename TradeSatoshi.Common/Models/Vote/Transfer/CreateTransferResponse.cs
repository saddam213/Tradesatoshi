using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public class CreateTransferResponse : ITradeResponse
	{
		public CreateTransferResponse(string error = null)
		{
			Error = error;
		}

		public string Error { get; set; }
		public bool HasError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}
	}
}
