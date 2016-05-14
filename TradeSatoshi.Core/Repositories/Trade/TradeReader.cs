using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Base.Extensions;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Exchange;
using TradeSatoshi.Common.Repositories.Trade;
using TradeSatoshi.Common.Services;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Core.Helpers;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Trade
{
	public class TradeReader : ITradeReader
	{
		public ICacheService CacheService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetUserTradeDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Trade
					.Where(x => x.UserId == userId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending) && x.TradePair.Status != TradePairStatus.Closed)
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
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetUserTradeHistoryDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TradeHistory
					.Where(x => (x.UserId == userId || x.ToUserId == userId) && x.TradePair.Status != TradePairStatus.Closed)
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
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetTradeDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Trade
					.Where(x => x.TradePair.Status != TradePairStatus.Closed)
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
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetTradeHistoryDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TradeHistory
					.Where(x => x.TradePair.Status != TradePairStatus.Closed)
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
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetTradePairTradeHistoryDataTable(DataTablesModel model, int tradePairId)
		{
			var cacheResult = await CacheService.GetOrSetASync(TradeCacheKeys.GetTradeHistoryKey(tradePairId), TimeSpan.FromMinutes(60), async () =>
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var query = context.TradeHistory
						.Where(x => x.TradePairId == tradePairId && x.TradePair.Status != TradePairStatus.Closed)
						.Select(x => new TradeHistoryDataTableModel
						{
							Amount = x.Amount,
							Rate = x.Rate,
							Timestamp = x.Timestamp,
							TradeHistoryType = x.TradeHistoryType,
						}).OrderByDescending(x => x.Timestamp).Take(500);
					return await query.GetDataTableResultNoLockAsync(model);
				}
			});
			return cacheResult;
		}

		public async Task<DataTablesResponse> GetTradePairOrderBookDataTable(DataTablesModel model, int tradePairId, TradeType tradeType)
		{
			var cacheKey = tradeType == TradeType.Buy
			? TradeCacheKeys.GetOpenBuyOrdersKey(tradePairId)
			: TradeCacheKeys.GetOpenSellOrdersKey(tradePairId);
			var cacheResult = await CacheService.GetOrSetASync(cacheKey, TimeSpan.FromMinutes(60), async () =>
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var query = context.Trade
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

					return await query.GetDataTableResultNoLockAsync(model);
				}
			});
			return cacheResult;
		}

		public async Task<DataTablesResponse> GetTradePairUserOpenOrdersDataTable(DataTablesModel model, int tradePairId, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Trade
					.Where(x => x.TradePairId == tradePairId && x.UserId == userId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
					.Select(x => new TradeOpenOrderModel
					{
						Id = x.Id,
						TradeType = x.TradeType,
						Rate = x.Rate,
						Amount = x.Amount,
						Remaining = x.Remaining,
						Timestamp = x.Timestamp
					}).OrderByDescending(x => x.Id);

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetUserTradePairTradeHistoryDataTable(DataTablesModel model, int tradePairId, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TradeHistory
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
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}


		public async Task<TradePairInfoModel> GetTradePairInfo(int tradePairId, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var tradePair = await context.TradePair
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.FirstOrDefaultNoLockAsync(x => x.Id == tradePairId);

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
					var balances = await context.Balance.Where(x => x.UserId == userId && (x.CurrencyId == tradePair.CurrencyId1 || x.CurrencyId == tradePair.CurrencyId2)).ToListNoLockAsync();
					var balance1 = balances.FirstOrDefault(x => x.CurrencyId == tradePair.CurrencyId1);
					var balance2 = balances.FirstOrDefault(x => x.CurrencyId == tradePair.CurrencyId2);
					model.Balance = balance1?.Avaliable ?? 0m;
					model.BaseBalance = balance2?.Avaliable ?? 0m;
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
					Fee = model.Fee,
					MinTrade = model.MinTrade
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
			var cacheResult = await CacheService.GetOrSetASync(TradeCacheKeys.GetTradePairChartKey(tradePairId), TimeSpan.FromMinutes(5), async () =>
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
								var firstOrDefault = tradePairData.FirstOrDefault();
								if (firstOrDefault != null) lastClose = firstOrDefault.Rate;
							}
							chartData.Add(new ChartDataModel(date.ToJavaTime(), lastClose, lastClose, lastClose, lastClose, 0));
							continue;
						}

						var max = data.Max(x => x.Rate);
						lastClose = data.Last().Rate;
						chartData.Add(new ChartDataModel(date.ToJavaTime(), data.First().Rate, max, data.Min(x => x.Rate), lastClose, data.Sum(x => x.Amount * x.Rate)));
					}

					return MapChartData(chartData);
				}
			});
			return cacheResult;
		}

		public async Task<ChartDepthDataViewModel> GetTradePairDepth(int tradePairId)
		{
			var cacheResult = await CacheService.GetOrSetASync(TradeCacheKeys.GetTradePairDepthKey(tradePairId), TimeSpan.FromMinutes(5), async () =>
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var trades = await context.Trade.Where(x => x.TradePairId == tradePairId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending)).ToListAsync();
					var buyTrades = trades.Where(x => x.TradeType == TradeType.Buy).ToList();
					var sellTrades = trades.Where(x => x.TradeType == TradeType.Sell).ToList();
					var sellData = new List<decimal[]>();
					var buyData = new List<decimal[]>();


					var buyTotal = 0m;
					foreach (var group in buyTrades.GroupBy(x => x.Rate).OrderByDescending(x => x.Key))
					{
						buyTotal += group.Sum(t => t.Remaining * t.Rate);
						buyData.Insert(0, new[] { group.Key, buyTotal });
					}

					var sellTotal = 0m;
					foreach (var group in sellTrades.GroupBy(x => x.Rate).OrderBy(x => x.Key))
					{
						sellTotal += group.Sum(t => t.Remaining * t.Rate);
						sellData.Add(new[] { group.Key, sellTotal });
					}

					return new ChartDepthDataViewModel
					{
						BuyDepth = new List<decimal[]>(buyData),
						SellDepth = new List<decimal[]>(sellData)
					};
				}
			});
			return cacheResult;
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

	//series: [{
	//	name: 'Buy',
	//	data: [[0.001, 0], [0.001, 0], [0.001, 0], [0.001, 0], [0.001, 4000.00000000], [0.002, 3000.00000000], [0.003, 2000.00000000], [0.004, 1000.00000000], [0.0045, 100.00000000]]
	//},
	//{
	//	name: 'Sell',
	//	data: [[0.008, 4000.00000000], [0.007, 3000.00000000], [0.006, 2000.00000000], [0.005, 1000.00000000], [0.0045, 100.00000000],[0.005, 0], [0.005, 0], [0.005, 0], [0.005, 0]]
	//}]
}