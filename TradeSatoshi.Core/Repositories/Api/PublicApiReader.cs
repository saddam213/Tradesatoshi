using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Repositories.Api;
using TradeSatoshi.Common.Services;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Repositories.Api
{
	public class PublicApiReader : IPublicApiReader
	{
		public ICacheService CacheService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IApiResult<List<ApiCurrency>>> GetCurrencies()
		{
			try
			{
				var cacheResult = await CacheService.GetOrSetASync<List<ApiCurrency>>(ApiCacheKeys.GetCurrenciesKey(), TimeSpan.FromMinutes(10), async () =>
				{
					using (var context = DataContextFactory.CreateContext())
					{
						var currencies = await context.Currency
												.Where(x => x.IsEnabled)
												.OrderBy(x => x.Symbol)
												.Select(c => new ApiCurrency
												{
													Currency = c.Symbol,
													CurrencyLong = c.Name,
													Status = c.Status.ToString(),
													MinConfirmation = c.MinConfirmations,
													TxFee = c.WithdrawFee
												}).ToListNoLockAsync();

						return currencies;
					}
				});

				return new ApiResult<List<ApiCurrency>>(true, cacheResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<List<ApiCurrency>>(ex);
			}
		}

		public async Task<IApiResult<List<ApiMarketHistory>>> GetMarketHistory(string market, int count)
		{
			try
			{
				var cacheResult = await CacheService.GetOrSetASync<List<ApiMarketHistory>>(ApiCacheKeys.GetMarketHistoryKey(market, count), 15, async () =>
				{
					using (var context = DataContextFactory.CreateContext())
					{
						var history = await context.TradeHistory
												.Where(t => t.TradePair.Name == market)
												.OrderByDescending(x => x.Id)
												.Select(c => new ApiMarketHistory
												{
													Id = c.Id,
													OrderType = c.TradeHistoryType.ToString(),
													Price = c.Rate,
													Quantity = c.Amount,
													TimeStamp = c.Timestamp
												})
												.Take(count)
												.ToListNoLockAsync();

						return history;
					}
				});

				return new ApiResult<List<ApiMarketHistory>>(true, cacheResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<List<ApiMarketHistory>>(ex);
			}
		}

		public async Task<IApiResult<List<ApiMarketSummary>>> GetMarketSummaries()
		{
			try
			{
				var cacheResult = await CacheService.GetOrSetASync<List<ApiMarketSummary>>(ApiCacheKeys.GetMarketSummariesKey(), 15, async () =>
				{
					using (var context = DataContextFactory.CreateContext())
					{
						// TODO: Cache data
						var timeLast = DateTime.UtcNow.AddHours(-24);
						var query =
												from tradePair in context.TradePair.Where(t => t.Status != TradePairStatus.Closed)
												from ticker in context.Trade.Where(x => x.TradePairId == tradePair.Id && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
														.GroupBy(x => x.TradePairId)
														.Select(c => new
														{
															Ask = c.Where(x => x.TradeType == TradeType.Sell).Max(x => x.Rate),
															Bid = c.Where(x => x.TradeType == TradeType.Buy).Max(x => x.Rate),
															OpenBuyOrders = c.Count(x => x.TradeType == TradeType.Buy),
															OpenSellOrders = c.Count(x => x.TradeType == TradeType.Sell)
														}).DefaultIfEmpty()
												from history in context.TradeHistory
														.Where(x => x.Timestamp > timeLast && x.TradePairId == tradePair.Id)
														.GroupBy(x => x.TradePairId)
														.Select(c => new
														{
															High = c.Max(x => x.Rate),
															Low = c.Min(x => x.Rate),
															Volume = c.Sum(x => x.Amount),
															BaseVolume = c.Select(x => new ApiVolume { Rate = x.Rate, Amount = x.Amount }).ToList()
														}).DefaultIfEmpty()
												select new ApiMarketSummary
												{
													Market = tradePair.Name,
													High = (decimal?)history.High ?? 0,
													Low = (decimal?)history.Low ?? 0,
													Volume = (decimal?)history.Volume ?? 0,
													BaseVolumeList = history.BaseVolume,
													Ask = (decimal?)ticker.Ask ?? 0,
													Bid = (decimal?)ticker.Bid ?? 0,
													Last = (decimal?)tradePair.LastTrade ?? 0,
													OpenBuyOrders = (int?)ticker.OpenBuyOrders ?? 0,
													OpenSellOrders = (int?)ticker.OpenSellOrders ?? 0
												};

						return await query.ToListNoLockAsync();
					}
				});

				return new ApiResult<List<ApiMarketSummary>>(true, cacheResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<List<ApiMarketSummary>>(ex);
			}
		}

		public async Task<IApiResult<ApiMarketSummary>> GetMarketSummary(string market)
		{
			try
			{
				var cacheResult = await CacheService.GetOrSetASync<ApiMarketSummary>(ApiCacheKeys.GetMarketSummaryKey(market), 15, async () =>
				{
					using (var context = DataContextFactory.CreateContext())
					{
						// TODO: Cache data
						var timeLast = DateTime.UtcNow.AddHours(-24);
						var query =
												from tradePair in context.TradePair.Where(t => t.Status != TradePairStatus.Closed)
												from ticker in context.Trade.Where(x => x.TradePairId == tradePair.Id && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
														.GroupBy(x => x.TradePairId)
														.Select(c => new
														{
															Ask = c.Where(x => x.TradeType == TradeType.Sell).Max(x => x.Rate),
															Bid = c.Where(x => x.TradeType == TradeType.Buy).Max(x => x.Rate),
															OpenBuyOrders = c.Count(x => x.TradeType == TradeType.Buy),
															OpenSellOrders = c.Count(x => x.TradeType == TradeType.Sell)
														}).DefaultIfEmpty()
												from history in context.TradeHistory
														.Where(x => x.Timestamp > timeLast && x.TradePairId == tradePair.Id)
														.GroupBy(x => x.TradePairId)
														.Select(c => new
														{
															High = c.Max(x => x.Rate),
															Low = c.Min(x => x.Rate),
															Volume = c.Sum(x => x.Amount),
															BaseVolume = c.Select(x => new ApiVolume { Rate = x.Rate, Amount = x.Amount }).ToList()
														}).DefaultIfEmpty()
												where tradePair.Name == market
												select new ApiMarketSummary
												{
													Market = tradePair.Name,
													High = (decimal?)history.High ?? 0,
													Low = (decimal?)history.Low ?? 0,
													Volume = (decimal?)history.Volume ?? 0,
													BaseVolumeList = history.BaseVolume,
													Ask = (decimal?)ticker.Ask ?? 0,
													Bid = (decimal?)ticker.Bid ?? 0,
													Last = (decimal?)tradePair.LastTrade ?? 0,
													OpenBuyOrders = ticker.OpenBuyOrders,
													OpenSellOrders = ticker.OpenSellOrders
												};

						return await query.FirstOrDefaultNoLockAsync();
					}
				});

				return new ApiResult<ApiMarketSummary>(true, cacheResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiMarketSummary>(ex);
			}
		}

		public async Task<IApiResult<ApiOrderBookResponse>> GetOrderBook(string market, string type, int depth)
		{
			try
			{
				var cacheResult = await CacheService.GetOrSetASync<ApiOrderBookResponse>(ApiCacheKeys.GetOrderBookKey(market, type, depth), 15, async () =>
				{
					using (var context = DataContextFactory.CreateContext())
					{
						var orderBookData = await context.Trade
												.Where(x => x.TradePair.Name == market && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
												.GroupBy(x => new { x.TradeType, x.Rate })
												.Select(c => new
												{
													Type = c.Key.TradeType,
													Order = new ApiOrderBookItem
													{
														Rate = c.Key.Rate,
														Quantity = c.Sum(x => x.Remaining)
													}
												}).ToListNoLockAsync();

						var buys = new List<ApiOrderBookItem>();
						var sells = new List<ApiOrderBookItem>();
						if (type.Equals("both", StringComparison.OrdinalIgnoreCase) || type.Equals("buy", StringComparison.OrdinalIgnoreCase))
						{
							buys = orderBookData.Where(x => x.Type == TradeType.Buy)
													.Select(x => x.Order)
													.OrderByDescending(x => x.Rate)
													.Take(depth)
													.ToList();
						}

						if (type.Equals("both", StringComparison.OrdinalIgnoreCase) || type.Equals("sell", StringComparison.OrdinalIgnoreCase))
						{
							sells = orderBookData.Where(x => x.Type == TradeType.Sell)
													.Select(x => x.Order)
													.OrderBy(x => x.Rate)
													.Take(depth)
													.ToList();
						}

						var orderBook = new ApiOrderBookResponse
						{
							Buy = buys,
							Sell = sells
						};

						return orderBook;
					}
				});

				return new ApiResult<ApiOrderBookResponse>(true, cacheResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiOrderBookResponse>(ex);
			}
		}

		public async Task<IApiResult<ApiTicker>> GetTicker(string market)
		{
			try
			{
				var cacheResult = await CacheService.GetOrSetASync<ApiTicker>(ApiCacheKeys.GetTickerKey(market), 15, async () =>
				{
					using (var context = DataContextFactory.CreateContext())
					{
						// TODO: Cache data
						var timeLast = DateTime.UtcNow.AddHours(-24);
						var query = from tradepair in context.TradePair.Where(t => t.Name == market)
												from trade in context.Trade.Where(t => t.TradePairId == tradepair.Id && (t.Status == TradeStatus.Partial || t.Status == TradeStatus.Pending))
																.GroupBy(g => g.TradePairId)
																.Select(x => new
																{
																	Ask = x.Where(j => j.TradeType == TradeType.Sell).Max(j => j.Rate),
																	Bid = x.Where(j => j.TradeType == TradeType.Buy).Max(j => j.Rate),
																})
												select new ApiTicker
												{
													Market = tradepair.Name,
													Ask = (decimal?)trade.Ask ?? 0,
													Bid = (decimal?)trade.Bid ?? 0,
													Last = tradepair.LastTrade
												};

						return await query.FirstOrDefaultNoLockAsync();
					}
				});

				return new ApiResult<ApiTicker>(true, cacheResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiTicker>(ex);
			}
		}
	}
}