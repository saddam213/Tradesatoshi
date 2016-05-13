using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Repositories.Api
{
	public interface IPublicApiReader
	{
		Task<IApiResult<List<ApiCurrency>>> GetCurrencies();
		Task<IApiResult<ApiTicker>> GetTicker(string market);
		Task<IApiResult<List<ApiMarketHistory>>> GetMarketHistory(string market, int count);
		Task<IApiResult<ApiMarketSummary>> GetMarketSummary(string market);
		Task<IApiResult<List<ApiMarketSummary>>> GetMarketSummaries();
		Task<IApiResult<ApiOrderBookResponse>> GetOrderBook(string market, string type, int depth);
	}
}