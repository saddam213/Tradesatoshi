using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Currency
{
	public interface ICurrencyWriter
	{
		Task<WriterResult<bool>> CreateCurrency(string userId, CreateCurrencyModel model);
		Task<WriterResult<bool>> UpdateCurrency(string userId, UpdateCurrencyModel model);
	}
}