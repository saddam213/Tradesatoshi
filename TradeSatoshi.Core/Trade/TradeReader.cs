using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Core.Services;
using TradeSatoshi.Core.Heplers;
using System.Data.Entity;

namespace TradeSatoshi.Core.Trade
{
	public class TradeReader : ITradeReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public DataTablesResponse GetTradeDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Trade.Include(t => t.TradePair.Currency1).Include(t => t.TradePair.Currency2).Where(x => x.UserId == userId).Select(x => new TradeModel
				{
					Amount = x.Amount,
					Fee = x.Fee,
					Id = x.Id,
					IsApi = x.IsApi,
					Rate = x.Rate,
					Timestamp = x.Timestamp,
					Status = x.Status,
					TradePair = x.TradePair.Currency1.Symbol + "/" + x.TradePair.Currency2.Symbol,
					Remaining = x.Remaining,
					TradeType = x.TradeType
				});
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetTradeHistoryDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TradeHistory.Include(t => t.TradePair.Currency1).Include(t => t.TradePair.Currency2).Where(x => x.UserId == userId || x.ToUserId == userId).Select(x => new TradeHistoryModel
				{
					Amount = x.Amount,
					Fee = x.Fee,
					Id = x.Id,
					IsApi = x.IsApi,
					Rate = x.Rate,
					Timestamp = x.Timestamp,
					TradeHistoryType = x.TradeHistoryType,
					TradePair = x.TradePair.Currency1.Symbol + "/" + x.TradePair.Currency2.Symbol
				});
				return query.GetDataTableResult(model);
			}
		}
	}
}
