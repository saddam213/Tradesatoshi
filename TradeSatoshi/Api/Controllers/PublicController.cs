using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TradeSatoshi.Common.Currency;

namespace TradeSatoshi.Web.Api.Controllers
{
	public class PublicController : ApiController
	{
		public ICurrencyReader CurrencyReader { get; set; }

		public async Task<IEnumerable<CurrencyModel>> GetCoins()
		{
			return await CurrencyReader.GetCurrencies();
		}
	}
}