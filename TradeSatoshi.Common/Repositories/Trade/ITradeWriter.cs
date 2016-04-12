using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Trade
{
	public interface ITradeWriter
	{
		Task<IWriterResult<bool>> CreateTrade(string userId, CreateTradeModel tradeItem);
		Task<IWriterResult<bool>> CancelTrade(string userId, CancelTradeModel tradeItem);
	}
}