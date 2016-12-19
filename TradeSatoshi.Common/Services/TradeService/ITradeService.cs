using System.Threading.Tasks;
using TradeSatoshi.Common.Faucet;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Transfer;

namespace TradeSatoshi.Common.Services.TradeService
{
	public interface ITradeService
	{
		Task<CreateTradeResponse> QueueTrade(CreateTradeModel tradeItem);
		Task<CancelTradeResponse> QueueCancel(CancelTradeModel tradeItem);
		Task<CreateTransferResponse> QueueTransfer(CreateTransferModel tradeItem);
		Task<CreateFaucetPaymentResponse> QueueFaucetPayment(CreateFaucetPaymentModel tradeItem);
	}
}