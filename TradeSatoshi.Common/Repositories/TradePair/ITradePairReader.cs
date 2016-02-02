using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;

namespace TradeSatoshi.Common.TradePair
{
	public interface ITradePairReader
	{
		Task<UpdateTradePairModel> GetTradePairUpdate(int tradePairId);
		Task<TradePairModel> GetTradePair(int tradePairId);
		Task<List<TradePairModel>> GetTradePairs();
		DataTablesResponse GetTradePairDataTable(DataTablesModel model);
	}
}
