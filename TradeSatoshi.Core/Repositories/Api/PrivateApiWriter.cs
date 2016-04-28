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

namespace TradeSatoshi.Core.Repositories.Api
{
	public class PrivateApiWriter : IPrivateApiWriter
	{
		public ITradeService TradeService { get; set; }
		public IAddressWriter AddressWriter { get; set; }
		public IWithdrawWriter WithdrawWriter { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<ApiResult<CancelOrderResponse>> CancelOrder(string userId, int orderId)
		{
			try
			{
				var result = await TradeService.QueueTradeItem(new CancelTradeModel
				{
					CancelType = Enums.CancelTradeType.Trade,
					IsApi = true,
					TradeId = orderId,
					UserId = userId
				});


				return new ApiResult<CancelOrderResponse>(true, "");
			}
			catch (Exception ex)
			{
				return new ApiResult<CancelOrderResponse>(ex);
			}
		}

		public async Task<ApiResult<List<CancelOrderResponse>>> CancelOrders(string userId, string market, string type)
		{
			try
			{
				//var result = await TradeService.QueueTradeItem(new CancelTradeModel
				//{
				//	CancelType = Enums.CancelTradeType.Trade,
				//	IsApi = true,
				//	TradeId = orderId,
				//	UserId = userId
				//});
				return new ApiResult<List<CancelOrderResponse>>(false, "Not Implemented");
			}
			catch (Exception ex)
			{
				return new ApiResult<List<CancelOrderResponse>>(ex);
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

		public async Task<ApiResult<ApiOrderResponse>> SubmitOrder(string userId, string market, string type, decimal amount, decimal price)
		{
			try
			{
				return new ApiResult<ApiOrderResponse>(false, "Not Implemented");
			}
			catch (Exception ex)
			{
				return new ApiResult<ApiOrderResponse>(ex);
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
					Id = result.Data
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
