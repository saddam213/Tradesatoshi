using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Address
{
	public interface IAddressWriter
	{
		Task<IWriterResult<string>> GenerateAddress(string userId, int currencyId);
		Task<IWriterResult<string>> GenerateAddress(string userId, string currency);
	}
}