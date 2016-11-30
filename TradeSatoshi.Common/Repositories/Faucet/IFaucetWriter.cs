using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Faucet
{
	public interface IFaucetWriter
	{
		Task<WriterResult<bool>> Claim(string userId, int faucetId);
	}
}
