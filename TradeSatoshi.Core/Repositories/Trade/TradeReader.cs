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
using TradeSatoshi.Base.Extensions;
using TradeSatoshi.Common.Exchange;

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
					.Where(x => x.UserId == userId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
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
					.Where(x => x.TradePairId == tradePairId)
					.Select(x => new TradeHistoryDataTableModel
					{
						Amount = x.Amount,
						Rate = x.Rate,
						Timestamp = x.Timestamp,
						TradeHistoryType = x.TradeHistoryType,
					}).OrderByDescending(x => x.Timestamp);
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


		public async Task<TradePairInfoModel> GetTradePairInfo(int tradePairId, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var tradePair = await context.TradePair
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.FirstOrDefaultAsync(x => x.Id == tradePairId);

				var model = new TradePairInfoModel
				{
					TradePairId = tradePair.Id,
					Fee = tradePair.Currency2.TradeFee,
					MinTrade = tradePair.Currency2.MinBaseTrade,
					Symbol = tradePair.Currency1.Symbol,
					BaseSymbol = tradePair.Currency2.Symbol,
				};

				if (!string.IsNullOrEmpty(userId))
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

		public async Task<TradePairExchangeModel> GetTradePairExchange(int tradePairId, string userId)
		{
			var model = await GetTradePairInfo(tradePairId, userId);
			return new TradePairExchangeModel
			{
				TradePairId = model.TradePairId,
				Symbol = model.Symbol,
				BaseSymbol = model.BaseSymbol,
				Balance = model.Balance,
				BaseBalance = model.BaseBalance,
				BuyModel = new CreateTradeModel
				{
					TradePairId = model.TradePairId,
					TradeType = TradeType.Buy,
					Symbol = model.Symbol,
					BaseSymbol = model.BaseSymbol,
					Fee = 0.2m,
					MinTrade = 0.00002000m
				},
				SellModel = new CreateTradeModel
				{
					TradePairId = model.TradePairId,
					TradeType = TradeType.Sell,
					Symbol = model.Symbol,
					BaseSymbol = model.BaseSymbol,
					Fee = model.Fee,
					MinTrade = model.MinTrade
				},
			};
		}

		public async Task<ChartDataViewModel> GetTradePairChart(int tradePairId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var tradePairData = await context.TradeHistory
				.Where(x => x.TradePairId == tradePairId)
				.Select(x => new
				{
					Amount = x.Amount,
					Rate = x.Rate,
					Timestamp = x.Timestamp
				}).ToListAsync();

				if (tradePairData.IsNullOrEmpty())
					return MapChartData(new[] { new ChartDataModel(DateTime.UtcNow.ToJavaTime(), 0, 0, 0, 0, 0) });

				var chartData = new List<ChartDataModel>();
				var start = tradePairData.Min(x => x.Timestamp);
				var finish = DateTime.UtcNow;
				var totalhours = (finish - start).TotalHours;
				var lastClose = 0m;
				var interval = TimeSpan.FromHours(1);
				var tickData = tradePairData.GroupBy(s => s.Timestamp.Ticks / interval.Ticks);
				for (int i = 0; i < totalhours; i++)
				{
					var date = start.AddHours(i);
					var data = tickData.FirstOrDefault(x => x.Any(c => c.Timestamp > date && c.Timestamp < date.Add(interval)));
					if (data.IsNullOrEmpty())
					{
						if (lastClose == 0)
						{
							lastClose = tradePairData.FirstOrDefault().Rate;
						}
						chartData.Add(new ChartDataModel(date.ToJavaTime(), lastClose, lastClose, lastClose, lastClose, 0));
						continue;
					}

					var max = data.Max(x => x.Rate);
					lastClose = data.Last().Rate;
					chartData.Add(new ChartDataModel(date.ToJavaTime(), data.First().Rate, max, data.Min(x => x.Rate), lastClose, data.Sum(x => x.Amount)));
				}

				return MapChartData(chartData);
			}
		}

		private ChartDataViewModel MapChartData(IEnumerable<ChartDataModel> chartData)
		{
			return new ChartDataViewModel
			{
				Candle = chartData.Select(x => new[]
					{ 
						x.Timestamp,
						x.Open,
						x.High,
						x.Low,
						x.Close
					}).ToList(),
				Volume = chartData.Select(x => new[]
					{ 
						x.Timestamp,
						x.Volume 
					}).ToList()
			};
		}
	}


}
