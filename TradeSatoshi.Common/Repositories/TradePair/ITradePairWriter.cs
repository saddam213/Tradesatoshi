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

namespace TradeSatoshi.Common.TradePair
{
	public interface ITradePairWriter
	{
		Task<WriterResult<bool>> CreateTradePair(string userId, CreateTradePairModel model);
		Task<WriterResult<bool>> UpdateTradePair(string userId, UpdateTradePairModel model);
	}
}
