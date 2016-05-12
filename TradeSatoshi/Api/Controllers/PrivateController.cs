using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TradeSatoshi.Common.Repositories.Api;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Web.Api.Authentication;

namespace TradeSatoshi.Web.Api.Controllers
{
	[ApiAuthentication]
	public class PrivateController : ApiController
	{
		public IPrivateApiReader PrivateApiReader { get; set; }
		public IPrivateApiWriter PrivateApiWriter { get; set; }

		[HttpPost]
		public async Task<ApiResult<ApiOrderResponse>> SubmitOrder(ApiSubmitOrderRequest request)
		{
			return await PrivateApiWriter.SubmitOrder(User.Identity.Name, request.Market, request.Type, request.Amount, request.Price);
		}

		[HttpPost]
		public async Task<ApiResult<CancelOrderResponse>> CancelOrder(ApiCancelOrderRequest request)
		{
			return await PrivateApiWriter.CancelOrder(User.Identity.Name, request.Type, request.OrderId, request.Market);
		}

		[HttpPost]
		public async Task<ApiResult<ApiOrderResponse>> GetOrder(ApiOrderRequest request)
		{
			return await PrivateApiReader.GetOrder(User.Identity.Name, request.OrderId);
		}

		[HttpPost]
		public async Task<ApiResult<List<ApiOrderResponse>>> GetOrders(ApiOrdersRequest request)
		{
			return await PrivateApiReader.GetOrders(User.Identity.Name, request.Market, request.Count);
		}

		[HttpPost]
		public async Task<ApiResult<List<ApiTradeResponse>>> GetTradeHistory(ApiTradeRequest request)
		{
			return await PrivateApiReader.GetTradeHistory(User.Identity.Name, request.Market, request.Count);
		}

		[HttpPost]
		public async Task<ApiResult<ApiBalanceResponse>> GetBalance(ApiBalanceRequest request)
		{
			return await PrivateApiReader.GetBalance(User.Identity.Name, request.Currency);
		}

		[HttpPost]
		public async Task<ApiResult<List<ApiBalanceResponse>>> GetBalances()
		{
			return await PrivateApiReader.GetBalances(User.Identity.Name);
		}

		[HttpPost]
		public async Task<ApiResult<List<ApiDepositResponse>>> GetDeposits(ApiDepositRequest request)
		{
			return await PrivateApiReader.GetDeposits(User.Identity.Name, request.Currency, request.Count);
		}

		[HttpPost]
		public async Task<ApiResult<List<ApiWithdrawResponse>>> GetWithdrawals(ApiWithdrawRequest request)
		{
			return await PrivateApiReader.GetWithdrawals(User.Identity.Name, request.Currency, request.Count);
		}

		[HttpPost]
		public async Task<ApiResult<ApiAddressResponse>> GenerateAddress(ApiAddressRequest request)
		{
			return await PrivateApiWriter.GenerateAddress(User.Identity.Name, request.Currency);
		}

		[HttpPost]
		public async Task<ApiResult<ApiSubmitWithdrawResponse>> SubmitWithdraw(ApiSubmitWithdrawRequest request)
		{
			return await PrivateApiWriter.SubmitWithdraw(User.Identity.Name, request.Currency, request.Address, request.Amount);
		}
	}
}