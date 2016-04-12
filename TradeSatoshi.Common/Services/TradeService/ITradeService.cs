using System.Threading.Tasks;
using TradeSatoshi.Common.Trade;

namespace TradeSatoshi.Common.Services.TradeService
{
	public interface ITradeService
	{
		Task<ITradeResponse> QueueTradeItem(ITradeItem tradeItem);
	}
}