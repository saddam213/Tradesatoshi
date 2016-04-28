using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Repositories.Api
{
	public interface IPrivateApiWriter
	{
		Task<ApiResult<ApiSubmitWithdrawResponse>> SubmitWithdraw(string userId, string currency, string address, decimal amount);
		Task<ApiResult<ApiAddressResponse>> GenerateAddress(string userId, string currency);
		Task<ApiResult<List<CancelOrderResponse>>> CancelOrders(string userId, string market, string type);
		Task<ApiResult<CancelOrderResponse>> CancelOrder(string userId, int orderId);
		Task<ApiResult<ApiOrderResponse>> SubmitOrder(string userId, string market, string type, decimal amount, decimal price);
	}
}