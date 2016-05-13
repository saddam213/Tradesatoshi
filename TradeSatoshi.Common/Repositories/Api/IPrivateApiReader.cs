using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Repositories.Api
{
	public interface IPrivateApiReader
	{
		Task<ApiResult<ApiOrderResponse>> GetOrder(string name, int orderId);
		Task<ApiResult<List<ApiOrderResponse>>> GetOrders(string name, string market, int count);
		Task<ApiResult<List<ApiTradeResponse>>> GetTradeHistory(string name, string market, int count);
		Task<ApiResult<ApiBalanceResponse>> GetBalance(string name, string currency);
		Task<ApiResult<List<ApiBalanceResponse>>> GetBalances(string name);
		Task<ApiResult<List<ApiDepositResponse>>> GetDeposits(string name, string currency, int count);
		Task<ApiResult<List<ApiWithdrawResponse>>> GetWithdrawals(string name, string currency, int count);
	}
}