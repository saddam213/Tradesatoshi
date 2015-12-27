using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Address
{
	public interface IAddressWriter
	{
		IWriterResult<string> GenerateAddress(string userId, int currencyId);
		Task<IWriterResult<string>> GenerateAddressAsync(string userId, int currencyId);
	}
}
