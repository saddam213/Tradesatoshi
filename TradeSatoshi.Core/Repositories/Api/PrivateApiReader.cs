using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Repositories.Api;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Repositories.Api
{
	public class PrivateApiReader : IPrivateApiReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<ApiResult<ApiBalanceResponse>> GetBalance(string userId, string currency)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var query = from currencyrow in context.Currency.Where(x => x.Symbol == currency)
											from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currencyrow.Id).DefaultIfEmpty()
											from address in context.Address.Where(a => a.UserId == userId && a.CurrencyId == currencyrow.Id && a.IsActive).DefaultIfEmpty()
											where currencyrow.IsEnabled
											select new ApiBalanceResponse
											{
												Currency = currencyrow.Symbol,
												CurrencyLong = currencyrow.Name,
												Address = address.AddressHash,
												HeldForTrades = (decimal?)balance.HeldForTrades ?? 0m,
												PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0m,
												Total = (decimal?)balance.Total ?? 0m,
												Unconfirmed = (decimal?)balance.Unconfirmed ?? 0m
											};
					var result = await query.FirstOrDefaultNoLockAsync();
					return new ApiResult<ApiBalanceResponse>(false, result);
				}
				return new ApiResult<ApiBalanceResponse>(false, "Not Implemented");
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiBalanceResponse>(ex);
			}
		}

		public async Task<ApiResult<List<ApiBalanceResponse>>> GetBalances(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var query = from currency in context.Currency
											from balance in context.Balance.Where(b => b.UserId == userId && b.CurrencyId == currency.Id).DefaultIfEmpty()
											from address in context.Address.Where(a => a.UserId == userId && a.CurrencyId == currency.Id && a.IsActive).DefaultIfEmpty()
											where currency.IsEnabled
											orderby currency.Name
											select new ApiBalanceResponse
											{
												Currency = currency.Symbol,
												CurrencyLong = currency.Name,
												Address = address.AddressHash,
												HeldForTrades = (decimal?)balance.HeldForTrades ?? 0m,
												PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0m,
												Total = (decimal?)balance.Total ?? 0m,
												Unconfirmed = (decimal?)balance.Unconfirmed ?? 0m
											};
					var results = await query.ToListNoLockAsync();
					return new ApiResult<List<ApiBalanceResponse>>(false, results);
				}
			}
			catch (Exception ex)
			{
				return new ApiResult<List<ApiBalanceResponse>>(ex);
			}
		}



		public async Task<ApiResult<ApiOrderResponse>> GetOrder(string userId, int orderId)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var results = await context.Trade
						.Where(x => x.Id == orderId && x.UserId == userId && x.TradePair.Status != TradePairStatus.Closed)
						.Select(x => new ApiOrderResponse
						{
							Amount = x.Amount,
							Fee = x.Fee,
							Id = x.Id,
							IsApi = x.IsApi,
							Rate = x.Rate,
							Timestamp = x.Timestamp,
							Status = x.Status.ToString(),
							Market = x.TradePair.Name,
							Remaining = x.Remaining,
							Type = x.TradeType.ToString()
						}).FirstOrDefaultNoLockAsync();
					return new ApiResult<ApiOrderResponse>(true, results);
				}
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiOrderResponse>(ex);
			}
		}

		public async Task<ApiResult<List<ApiOrderResponse>>> GetOrders(string userId, string market, int count)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var results = await context.Trade
						.Where(x => x.UserId == userId && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending) && x.TradePair.Status != TradePairStatus.Closed)
						.Where(x => market == "all" || x.TradePair.Name == market)
						.Take(count)
						.Select(x => new ApiOrderResponse
						{
							Amount = x.Amount,
							Fee = x.Fee,
							Id = x.Id,
							IsApi = x.IsApi,
							Rate = x.Rate,
							Timestamp = x.Timestamp,
							Status = x.Status.ToString(),
							Market = x.TradePair.Name,
							Remaining = x.Remaining,
							Type = x.TradeType.ToString()
						}).ToListNoLockAsync();
					return new ApiResult<List<ApiOrderResponse>>(false, results);
				}
			}
			catch (Exception ex)
			{
				return new ApiResult<List<ApiOrderResponse>>(ex);
			}
		}

		public async Task<ApiResult<List<ApiTradeResponse>>> GetTradeHistory(string userId, string market, int count)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var results = await context.TradeHistory
						.Where(x => (x.UserId == userId || x.ToUserId == userId) && x.TradePair.Status != TradePairStatus.Closed)
						.Where(x => market == "all" || x.TradePair.Name == market)
						.Take(count)
						.Select(x => new ApiTradeResponse
						{
							Amount = x.Amount,
							Fee = x.Fee,
							Id = x.Id,
							IsApi = x.IsApi,
							Rate = x.Rate,
							Timestamp = x.Timestamp,
							Type = x.TradeHistoryType.ToString(),
							Market = x.TradePair.Name
						}).ToListNoLockAsync();
					return new ApiResult<List<ApiTradeResponse>>(false, results);
				}
			}
			catch (Exception ex)
			{
				return new ApiResult<List<ApiTradeResponse>>(ex);
			}
		}

		public async Task<ApiResult<List<ApiDepositResponse>>> GetDeposits(string userId, string currency, int count)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var results = await context.Deposit
						.Where(x => x.UserId == userId && x.Currency.IsEnabled)
						.Where(x => currency == "all" || x.Currency.Symbol == currency)
						.OrderByDescending(x => x.Id)
						.Take(count)
						.Select(deposit => new ApiDepositResponse
						{
							Amount = deposit.Amount,
							Confirmations = deposit.Confirmations,
							Currency = deposit.Currency.Symbol,
							CurrencyLong = deposit.Currency.Name,
							Id = deposit.Id,
							TimeStamp = deposit.TimeStamp,
							Txid = deposit.Txid,
							Status = deposit.DepositStatus.ToString()
						}).ToListNoLockAsync();

					return new ApiResult<List<ApiDepositResponse>>(true, results);
				}

			}
			catch (Exception ex)
			{
				return new ApiResult<List<ApiDepositResponse>>(ex);
			}
		}

		public async Task<ApiResult<List<ApiWithdrawResponse>>> GetWithdrawals(string userId, string currency, int count)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var results = await context.Withdraw
						.Where(x => x.UserId == userId && x.Currency.IsEnabled)
						.Where(x => currency == "all" || x.Currency.Symbol == currency)
						.Take(count)
						.Select(withdraw => new ApiWithdrawResponse
						{
							Address = withdraw.Address,
							Amount = withdraw.Amount,
							Confirmations = withdraw.Confirmations,
							Currency = withdraw.Currency.Symbol,
							CurrencyLong = withdraw.Currency.Name,
							Fee = withdraw.Fee,
							Id = withdraw.Id,
							TimeStamp = withdraw.TimeStamp,
							Txid = withdraw.Txid,
							Status = withdraw.WithdrawStatus.ToString()
						}).ToListNoLockAsync();
					return new ApiResult<List<ApiWithdrawResponse>>(false, results);
				}
			}
			catch (Exception ex)
			{
				return new ApiResult<List<ApiWithdrawResponse>>(ex);
			}
		}
	}
}
