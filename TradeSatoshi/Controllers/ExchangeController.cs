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

namespace TradeSatoshi.Web.Controllers
{
	public class ExchangeController : BaseController
	{
		public ITradeWriter TradeWriter { get; set; }
		public ITradeReader TradeReader { get; set; }
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

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> CreateTrade(CreateTradeModel model)
		{
			if (!ModelState.IsValid)
				return PartialView("_CreateTradePartial", model);

			var result = await TradeWriter.CreateTrade(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return PartialView("_CreateTradePartial", model);

			return PartialView("_CreateTradePartial", model);
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> CancelTrade(int id, CancelTradeType cancelType)
		{
			var result = await TradeWriter.CancelTrade(User.Id(), new CancelTradeModel
			{
				TradeId = id,
				TradePairId = id,
				CancelType = cancelType
			});

			return Json(result);
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