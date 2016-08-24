using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Exchange;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Trade
{
	public interface ITradeReader
	{
		Task<DataTablesResponse> GetTradeDataTable(DataTablesModel model);
		Task<DataTablesResponse> GetTradeHistoryDataTable(DataTablesModel model);
		Task<DataTablesResponse> GetUserTradeDataTable(DataTablesModel model, string userId);
		Task<DataTablesResponse> GetUserTradeHistoryDataTable(DataTablesModel model, string userId);

		Task<DataTablesResponse> GetTradePairTradeHistoryDataTable(DataTablesModel model, int tradePairId);
		Task<DataTablesResponse> GetTradePairOrderBookDataTable(DataTablesModel model, int tradePairId, TradeType tradeType);

		Task<DataTablesResponse> GetTradePairUserOpenOrdersDataTable(DataTablesModel model, int tradePairId, string userId);
		Task<DataTablesResponse> GetUserTradePairTradeHistoryDataTable(DataTablesModel model, int tradePairId, string userId);

		Task<TradePairInfoModel> GetTradePairInfo(int tradePairId, string userId);
		Task<TradePairExchangeModel> GetTradePairExchange(int tradePairId, string userId);
		Task<ChartDataViewModel> GetTradePairChart(int tradePairId);
	}
}