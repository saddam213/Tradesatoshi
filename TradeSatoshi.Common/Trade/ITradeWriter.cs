using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Trade
{
	public interface ITradeWriter
	{
		Task<IWriterResult<bool>> CreateTradeAsync(ITradeItem tradeItem);
		Task<IWriterResult<bool>> CancelTradeAsync(ITradeItem tradeItem);
	}
}
