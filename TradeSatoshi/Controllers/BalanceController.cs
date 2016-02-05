using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Address;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.User;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class BalanceController : BaseController
	{
		public IBalanceReader BalanceReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IAddressWriter AddressWriter { get; set; }

		public async Task<ActionResult> Index()
		{
			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			return View(new BalanceViewModel
			{
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances)
			});
		}

		//[HttpPost]
		//public ActionResult GetBalances(DataTablesModel param)
		//{
		//	return DataTable(BalanceReader.GetUserBalanceDataTable(param, User.Id()));
		//}

		[HttpPost]
		public async Task<ActionResult> GetAddress(int currencyId)
		{
			var result = await AddressWriter.GenerateAddress(User.Id(), currencyId);
			if (!ModelState.IsWriterResultValid(result))
				return JsonError(result.FirstError);

			return JsonSuccess(result.Message);
		}
	}
}