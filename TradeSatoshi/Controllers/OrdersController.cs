using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Orders;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class OrdersController : BaseController
	{
		public IBalanceReader BalanceReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public ITradeReader TradeReader { get; set; }
		public ITradeWriter TradeWriter { get; set; }

		public async Task<ActionResult> Index()
		{
			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			return View(new OrdersViewModel
			{
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances)
			});
		}

		[HttpPost]
		public ActionResult GetTrades(DataTablesModel param)
		{
			return DataTable(TradeReader.GetUserTradeDataTable(param, User.Id()));
		}
	}
}