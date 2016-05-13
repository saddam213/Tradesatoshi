using System.Threading.Tasks;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Trade
{
	public class TradeWriter : ITradeWriter
	{
		public ITradeService TradeService { get; set; }
	
		public async Task<IWriterResult<bool>> CreateTrade(string userId, CreateTradeModel model)
		{
			model.UserId = userId;
			var result = await TradeService.QueueTrade(model);
			if (result.HasError)
				return WriterResult<bool>.ErrorResult(result.Error);

			return WriterResult<bool>.SuccessResult();
		}

		public async Task<IWriterResult<bool>> CreateTransfer(string userId, CreateTransferModel model)
		{
			model.UserId = userId;
			var result = await TradeService.QueueTransfer(model);
			if (result.HasError)
				return WriterResult<bool>.ErrorResult(result.Error);

			return WriterResult<bool>.SuccessResult();
		}

		public async Task<IWriterResult<bool>> CancelTrade(string userId, CancelTradeModel model)
		{
			model.UserId = userId;
			var result = await TradeService.QueueCancel(model);
			if (result.HasError)
				return WriterResult<bool>.ErrorResult(result.Error);

			return WriterResult<bool>.SuccessResult();
		}
		
	}
}