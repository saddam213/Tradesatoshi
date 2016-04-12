using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.TradePair
{
	public interface ITradePairWriter
	{
		Task<WriterResult<bool>> CreateTradePair(string userId, CreateTradePairModel model);
		Task<WriterResult<bool>> UpdateTradePair(string userId, UpdateTradePairModel model);
	}
}