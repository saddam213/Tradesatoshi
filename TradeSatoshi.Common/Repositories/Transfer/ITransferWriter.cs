using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Transfer
{
	public interface ITransferWriter
	{
		Task<IWriterResult<bool>> CreateTransfer(CreateTransferModel tradeItem);
	}
}