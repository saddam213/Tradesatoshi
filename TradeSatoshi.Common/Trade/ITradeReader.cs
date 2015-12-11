using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Trade
{
	public interface ITradeReader
	{
		DataTablesResponse GetTradeDataTable(DataTablesModel model, string userId);
		DataTablesResponse GetTradeHistoryDataTable(DataTablesModel model, string userId);
	}
}
