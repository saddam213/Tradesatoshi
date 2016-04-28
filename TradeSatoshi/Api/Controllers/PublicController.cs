using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TradeSatoshi.Common.Repositories.Api;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Web.Api.Controllers
{
	public class PublicController : ApiController
	{
		public IPublicApiReader PublicApiReader { get; set; }

		public async Task<IApiResult<List<ApiCurrency>>> GetCurrencies()
		{
			return await PublicApiReader.GetCurrencies();
		}

		public async Task<IApiResult<ApiTicker>> GetTicker(string market)
		{
			return await PublicApiReader.GetTicker(market);
		}

		public async Task<IApiResult<List<ApiMarketHistory>>> GetMarketHistory(string market, int count = 20)
		{
			return await PublicApiReader.GetMarketHistory(market, count);
		}

		public async Task<IApiResult<ApiMarketSummary>> GetMarketSummary(string market)
		{
			return await PublicApiReader.GetMarketSummary(market);
		}

		public async Task<IApiResult<List<ApiMarketSummary>>> GetMarketSummaries()
		{
			return await PublicApiReader.GetMarketSummaries();
		}

		public async Task<IApiResult<ApiOrderBookResponse>> GetOrderBook(string market, string type = "both", int depth = 20)
		{
			return await PublicApiReader.GetOrderBook(market, type, depth);
		}
	}
}