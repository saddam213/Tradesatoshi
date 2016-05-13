using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Repositories.Api
{
	public interface IPrivateApiWriter
	{
		Task<ApiResult<ApiSubmitWithdrawResponse>> SubmitWithdraw(string userId, string currency, string address, decimal amount);
		Task<ApiResult<ApiAddressResponse>> GenerateAddress(string userId, string currency);
		Task<ApiResult<CancelOrderResponse>> CancelOrder(string userId, TradeCancelType cancelType, int? orderId, string market);
		Task<ApiResult<ApiOrderResponse>> SubmitOrder(string userId, string market, string type, decimal amount, decimal price);
	}
}