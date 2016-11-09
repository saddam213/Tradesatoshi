using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.WalletService;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Services.WithdrawService
{
	public interface IWithdrawService
	{
		Task<IWriterResult<int>> QueueWithdraw(CreateWithdraw withdraw);
	}
}
