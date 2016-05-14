using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Address;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Repositories.Api;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Services.WalletService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Repositories.Api
{
	public class PrivateApiWriter : IPrivateApiWriter
	{
		public ITradeService TradeService { get; set; }
		public IAddressWriter AddressWriter { get; set; }
		public IWithdrawWriter WithdrawWriter { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<ApiResult<CancelOrderResponse>> CancelOrder(string userId, TradeCancelType cancelType, int? orderId, string market)
		{
			try
			{
				var result = await TradeService.QueueCancel(new CancelTradeModel
				{
					UserId = userId,
					Market = market,
					OrderId = orderId,
					CancelType = cancelType,
					IsApi = true
				});

				if (result.HasError)
					return new ApiResult<CancelOrderResponse>(false, result.Error);

				var response = new CancelOrderResponse
				{
					CanceledOrders = result.CanceledOrders
				};
				return new ApiResult<CancelOrderResponse>(true, response);
			}
			catch (Exception ex)
			{
				return new ApiResult<CancelOrderResponse>(ex);
			}
		}

		public async Task<ApiResult<ApiAddressResponse>> GenerateAddress(string userId, string currency)
		{
			try
			{
				var result = await AddressWriter.GenerateAddress(userId, currency);
				if (result.HasErrors)
					return new ApiResult<ApiAddressResponse>(false, result.FirstError);

				var apiResult = new ApiAddressResponse
				{
					Currency = currency,
					Address = result.Data
				};
				return new ApiResult<ApiAddressResponse>(true, apiResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiAddressResponse>(ex);
			}
		}

		public async Task<ApiResult<ApiSubmitOrderResponse>> SubmitOrder(string userId, string market, TradeType type, decimal amount, decimal price)
		{
			try
			{
				var result = await TradeService.QueueTrade(new CreateTradeModel
				{
					UserId = userId,
					Amount = amount,
					TradeType = type,
					Rate = price,
					Market = market,
					IsApi = true
				});

				if (result.HasError)
					return new ApiResult<ApiSubmitOrderResponse>(false, result.Error);

				var apiResult = new ApiSubmitOrderResponse
				{
					OrderId = result.TradeId,
					Filled = result.FilledTrades
				};
				return new ApiResult<ApiSubmitOrderResponse>(true, apiResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiSubmitOrderResponse>(ex);
			}
		}

		public async Task<ApiResult<ApiSubmitWithdrawResponse>> SubmitWithdraw(string userId, string currency, string address, decimal amount)
		{
			try
			{
				var result = await WithdrawWriter.CreateApiWithdraw(userId, currency, address, amount);
				if (result.HasErrors)
					return new ApiResult<ApiSubmitWithdrawResponse>(false, result.FirstError);

				var apiResult = new ApiSubmitWithdrawResponse
				{
					WithdrawalId = result.Data
				};
				return new ApiResult<ApiSubmitWithdrawResponse>(true, apiResult);
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiSubmitWithdrawResponse>(ex);
			}
		}
	}
}
