using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Withdraw
{
	public interface IWithdrawWriter
	{
		Task<IWriterResult<int>> CreateWithdraw(string userId, CreateWithdrawModel model);
		Task<IWriterResult<int>> CreateApiWithdraw(string userId, string currency, string address, decimal amount);

		Task<IWriterResult<bool>> ConfirmWithdraw(string userId, int withdrawId);

		Task<IWriterResult<bool>> CancelWithdraw(string userId, int withdrawId);
	}
}