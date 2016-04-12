using System.Threading.Tasks;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Trade
{
	public class TradeWriter : ITradeWriter
	{
		public ITradeService TradeService { get; set; }

		public async Task<IWriterResult<bool>> CreateTrade(string userId, CreateTradeModel model)
		{
			model.UserId = userId;
			var result = await TradeService.QueueTradeItem(model);
			return WriterResult<bool>.SuccessResult();
		}

		public async Task<IWriterResult<bool>> CancelTrade(string userId, CancelTradeModel model)
		{
			model.UserId = userId;
			var result = await TradeService.QueueTradeItem(model);
			return WriterResult<bool>.SuccessResult();
		}
	}
}