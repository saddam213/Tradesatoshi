using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Withdraw
{
	public interface IWithdrawWriter
	{
		IWriterResult<int> CreateWithdraw(string userId, CreateWithdrawModel model);
		Task<IWriterResult<int>> CreateWithdrawAsync(string userId, CreateWithdrawModel model);

		IWriterResult<bool> ConfirmWithdraw(string userId, int withdrawId);
		Task<IWriterResult<bool>> ConfirmWithdrawAsync(string userId, int withdrawId);

		IWriterResult<bool> CancelWithdraw(string userId, int withdrawId);
		Task<IWriterResult<bool>> CancelWithdrawAsync(string userId, int withdrawId);
	}
}
