using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Exchange;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Trade
{
	public interface ITradeReader
	{
		DataTablesResponse GetTradeDataTable(DataTablesModel model);
		DataTablesResponse GetTradeHistoryDataTable(DataTablesModel model);
		DataTablesResponse GetUserTradeDataTable(DataTablesModel model, string userId);
		DataTablesResponse GetUserTradeHistoryDataTable(DataTablesModel model, string userId);

		DataTablesResponse GetTradePairTradeHistoryDataTable(DataTablesModel model, int tradePairId);
		DataTablesResponse GetTradePairOrderBookDataTable(DataTablesModel model, int tradePairId, TradeType tradeType);

		DataTablesResponse GetTradePairUserOpenOrdersDataTable(DataTablesModel model, int tradePairId, string userId);
		DataTablesResponse GetUserTradePairTradeHistoryDataTable(DataTablesModel model, int tradePairId, string userId);

		Task<TradePairInfoModel> GetTradePairInfo(int tradePairId, string userId);
		Task<TradePairExchangeModel> GetTradePairExchange(int tradePairId, string userId);
		Task<ChartDataViewModel> GetTradePairChart(int tradePairId);
	}
}
