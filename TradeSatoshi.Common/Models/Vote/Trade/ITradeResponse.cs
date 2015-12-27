using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.NotificationService;

namespace TradeSatoshi.Common.Trade
{
	public interface ITradeResponse
	{
		bool HasError { get; }
		string Error { get; set; }
	}
}
