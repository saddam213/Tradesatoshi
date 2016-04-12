using System.Threading.Tasks;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Transfer
{
	public class TransferWriter : ITransferWriter
	{
		public ITradeService TradeService { get; set; }

		public async Task<IWriterResult<bool>> CreateTransfer(CreateTransferModel model)
		{
			var result = await TradeService.QueueTradeItem(model);
			return WriterResult<bool>.SuccessResult();
		}
	}
}