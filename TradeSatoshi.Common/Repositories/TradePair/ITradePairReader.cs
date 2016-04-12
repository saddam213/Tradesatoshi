using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

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