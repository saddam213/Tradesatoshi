using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.Status;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class StatusController : Controller
	{
		public IBalanceReader BalanceReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }

		public async Task<ActionResult> Index()
		{
			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			var currencyStatus = await CurrencyReader.GetCurrencyStatus();
			return View(new StatusViewModel
			{
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances),
				CurrencyStatus = new List<CurrencyStatusModel>(currencyStatus)
			});
		}
	}
}