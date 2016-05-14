using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Web.Helpers;
using TradeSatoshi.Common.Exchange;
using TradeSatoshi.Enums;
using TradeSatoshi.Common.TradePair;
using System.Collections.Generic;
using System.Linq;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Repositories.Api;

namespace TradeSatoshi.Web.Controllers
{
	public class ExchangeController : BaseController
	{
		public ITradeWriter TradeWriter { get; set; }
		public ITradeReader TradeReader { get; set; }
		public IPublicApiReader PublicApiReader { get; set; }
		public IBalanceReader BalanceReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index(string market)
		{
			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			return View(new ExchangeModel
			{
				TradePair = market,
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances)
			});
		}

		[HttpGet]
		public async Task<ActionResult> TradePair(int tradePairId)
		{
			return PartialView("_TradePairPartial", await TradeReader.GetTradePairExchange(tradePairId, User.Id()));
		}

		[HttpGet]
		public async Task<ActionResult> Summary()
		{
			return PartialView("_SummaryPartial", await PublicApiReader.GetMarketSummaries());
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> CreateTrade(CreateTradeModel model)
		{
			var result = await TradeWriter.CreateTrade(User.Id(), model);
			if (!result.Data)
				return JsonError();

			return JsonSuccess();
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> CancelTrade(CancelTradeModel model)
		{
			var result = await TradeWriter.CancelTrade(User.Id(), model);
			if (!result.Data)
				return JsonError();

			return JsonSuccess();
		}

		[HttpPost]
		public async Task<ActionResult> GetOrderBook(DataTablesModel param, int tradePairId, TradeType tradeType)
		{
			return DataTable(await TradeReader.GetTradePairOrderBookDataTable(param, tradePairId, tradeType));
		}

		[HttpPost]
		public async Task<ActionResult> GetMarketHistory(DataTablesModel param, int tradePairId)
		{
			return DataTable(await TradeReader.GetTradePairTradeHistoryDataTable(param, tradePairId));
		}

		[HttpGet]
		public async Task<ActionResult> GetTradePairChart(int tradePairId)
		{
			return Json(await TradeReader.GetTradePairChart(tradePairId), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public async Task<ActionResult> GetTradePairDepth(int tradePairId)
		{
			return Json(await TradeReader.GetTradePairDepth(tradePairId), JsonRequestBehavior.AllowGet);
		}
	}
}