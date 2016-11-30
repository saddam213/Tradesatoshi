using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Faucet;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	[AuthorizeSecurityRole(SecurityRole.Standard)]
	public class FaucetController : BaseController
	{
		public IBalanceReader BalanceReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IFaucetReader FaucetReader { get; set; }
		public IFaucetWriter FaucetWriter { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			return View(new FaucetViewModel
			{
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances),
			});
		}

		[HttpPost]
		public async Task<ActionResult> GetFaucets(DataTablesModel param)
		{
			return DataTable(await FaucetReader.GetFaucetDataTable(param, User.Id()));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Claim(int id)
		{
			var result = await FaucetWriter.Claim(User.Id(), id);
			if (!ModelState.IsWriterResultValid(result))
				return JsonError();

			return JsonSuccess();
		}
	}
}