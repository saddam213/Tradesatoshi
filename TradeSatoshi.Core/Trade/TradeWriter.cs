using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.NotificationService;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Core.Services;
using TradeSatoshi.Base.Extensions;

namespace TradeSatoshi.Core.Trade
{
	public class TradeWriter : ITradeWriter
	{
		public ITradeService TradeService { get; set; }
		public INotificationService NotificationService { get; set; }

		public async Task<IWriterResult<bool>> CreateTradeAsync(CreateTradeModel model)
		{
			var result = await TradeService.QueueTradeItem(model);
			return WriterResult<bool>.SuccessResult();
		}

		public async Task<IWriterResult<bool>> CancelTradeAsync(CancelTradeModel model)
		{
			var result = await TradeService.QueueTradeItem(model);
			return WriterResult<bool>.SuccessResult();
		}
	}
}
