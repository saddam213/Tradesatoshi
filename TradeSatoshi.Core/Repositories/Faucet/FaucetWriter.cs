using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Faucet;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Faucet
{
	public class FaucetWriter : IFaucetWriter
	{
		public Task<WriterResult<bool>> Claim(string userId, int faucetId)
		{
			throw new NotImplementedException();
		}
	}
}
