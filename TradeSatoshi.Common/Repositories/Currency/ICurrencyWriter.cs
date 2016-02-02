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

namespace TradeSatoshi.Common.Currency
{
	public interface ICurrencyWriter
	{
		Task<WriterResult<bool>> CreateCurrency(string userId, CreateCurrencyModel model);
		Task<WriterResult<bool>> UpdateCurrency(string userId, UpdateCurrencyModel model);
	}
}
