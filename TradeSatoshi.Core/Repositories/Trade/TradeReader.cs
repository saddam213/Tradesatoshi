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
using TradeSatoshi.Core.Helpers;
using System.Data.Entity;
using TradeSatoshi.Common;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Trade
{
	public class TradeReader : ITradeReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public DataTablesResponse GetUserTradeDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Trade
					.Include(t => t.TradePair.Currency1)
					.Include(t => t.TradePair.Currency2)
					.Where(x => x.UserId == userId)
					.Select(x => new TradeModel
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

		public DataTablesResponse GetUserTradeHistoryDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TradeHistory
					.Include(t => t.TradePair.Currency1)
					.Include(t => t.TradePair.Currency2)
					.Where(x => x.UserId == userId || x.ToUserId == userId)
					.Select(x => new TradeHistoryModel
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

		public DataTablesResponse GetTradeDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Trade
					.Include(t => t.TradePair.Currency1)
					.Include(t => t.TradePair.Currency2)
					.Select(x => new TradeModel
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

		public DataTablesResponse GetTradeHistoryDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TradeHistory
					.Include(t => t.TradePair.Currency1)
					.Include(t => t.TradePair.Currency2)
					.Select(x => new TradeHistoryModel
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

		public DataTablesResponse GetTradePairTradeHistoryDataTable(DataTablesModel model, int tradePairId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TradeHistory
					.Include(t => t.TradePair.Currency1)
					.Include(t => t.TradePair.Currency2)
					.Where(x => x.TradePairId == tradePairId)
					.Select(x => new TradeHistoryModel
					{
						Amount = x.Amount,
						Fee = x.Fee,
						Id = x.Id,
						IsApi = x.IsApi,
						Rate = x.Rate,
						Timestamp = x.Timestamp,
						TradeHistoryType = x.TradeHistoryType,
						TradePair = x.TradePair.Currency1.Symbol + "/" + x.TradePair.Currency2.Symbol
					}).OrderByDescending(x => x.Id);
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetTradePairOrderBookDataTable(DataTablesModel model, int tradePairId, TradeType tradeType)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Trade
					.Include(t => t.TradePair.Currency1)
					.Include(t => t.TradePair.Currency2)
					.Where(x => x.TradePairId == tradePairId && x.TradeType == tradeType && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
					.GroupBy(t => t.Rate)
					.Select(x => new TradeOrderBookModel
					{
						Rate = x.Key,
						Total = x.Sum(o => o.Remaining),
						//SumTotal = x.Sum(o => o.Remaining) * x.Key,
						//OrderCount = x.Count()
					});

				query = tradeType == TradeType.Buy
					? query.OrderByDescending(o => o.Rate)
					: query.OrderBy(x => x.Rate);

				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetTradePairUserOpenOrdersDataTable(DataTablesModel model, int tradePairId, string UserId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Trade
					.Include(t => t.TradePair.Currency1)
					.Include(t => t.TradePair.Currency2)
					.Where(x => x.TradePairId == tradePairId && x.UserId == UserId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
					.Select(x => new TradeOpenOrderModel
					{
						Id = x.Id,
						TradeType = x.TradeType,
						Rate = x.Rate,
						Amount = x.Amount,
						Remaining = x.Remaining,
						Timestamp = x.Timestamp
					}).OrderByDescending(x => x.Id);

				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetUserTradePairTradeHistoryDataTable(DataTablesModel model, int tradePairId, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TradeHistory
					.Include(t => t.TradePair.Currency1)
					.Include(t => t.TradePair.Currency2)
					.Where(x => x.TradePairId == tradePairId && x.UserId == userId)
					.Select(x => new TradeHistoryModel
					{
						Amount = x.Amount,
						Fee = x.Fee,
						Id = x.Id,
						IsApi = x.IsApi,
						Rate = x.Rate,
						Timestamp = x.Timestamp,
						TradeHistoryType = x.TradeHistoryType,
						TradePair = x.TradePair.Currency1.Symbol + "/" + x.TradePair.Currency2.Symbol
					}).OrderByDescending(x => x.Id);
				return query.GetDataTableResult(model);
			}
		}


		public TradePairInfoModel GetTradePairInfo(int tradePairId, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var tradePair = context.TradePair
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.FirstOrDefault(x => x.Id == tradePairId);

				var model = new TradePairInfoModel
				{
					TradePairId = tradePair.Id,
					Fee = tradePair.Currency2.TradeFee,
					MinTrade = tradePair.Currency2.MinBaseTrade,
					Symbol = tradePair.Currency1.Symbol,
					BaseSymbol = tradePair.Currency2.Symbol,
				};

				if(!string.IsNullOrEmpty(userId))
				{
					var balances = context.Balance.Where(x => x.UserId == userId && (x.CurrencyId == tradePair.CurrencyId1 || x.CurrencyId == tradePair.CurrencyId2)).ToList();
					var balance1 = balances.FirstOrDefault(x => x.CurrencyId == tradePair.CurrencyId1);
					var balance2 = balances.FirstOrDefault(x => x.CurrencyId == tradePair.CurrencyId2);
					model.Balance = balance1 != null ? balance1.Avaliable : 0m;
					model.BaseBalance = balance2 != null ? balance2.Avaliable : 0m;
				}

				return model;
			}
		}
	}
}
