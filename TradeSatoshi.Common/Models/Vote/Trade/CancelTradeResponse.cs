using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Trade
{
	public class CancelTradeResponse : ITradeResponse
	{
		public CancelTradeResponse()
		{
			CanceledOrders = new List<int>();
		}

		public CancelTradeResponse(string error)
			: base()
		{
			Error = error;
		}

		public List<int> CanceledOrders { get; set; }
		public string Error { get; set; }
		public bool HasError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}

		public void AddCanceledOrder(int tradeId)
		{
			CanceledOrders.Add(tradeId);
		}
	}
}
