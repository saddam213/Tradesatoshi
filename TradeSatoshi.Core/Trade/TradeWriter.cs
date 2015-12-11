using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Core.Services;

namespace TradeSatoshi.Core.Trade
{
	public class TradeWriter : ITradeWriter
	{
		public ITradeService TradeService { get; set; }

		public async Task<IWriterResult<bool>> CreateTradeAsync(ITradeItem tradeItem)
		{
			await TradeService.QueueTradeItem(tradeItem);
			return WriterResult<bool>.SuccessResult();
		}

		public async Task<IWriterResult<bool>> CancelTradeAsync(ITradeItem tradeItem)
		{
			await TradeService.QueueTradeItem(tradeItem);
			return WriterResult<bool>.SuccessResult();
		}
	}
}
