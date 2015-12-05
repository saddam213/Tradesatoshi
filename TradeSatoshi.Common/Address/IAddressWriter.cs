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
		IWriterResult GenerateAddress(string userId, int currencyId);
		Task<IWriterResult> GenerateAddressAsync(string userId, int currencyId);
	}
}
