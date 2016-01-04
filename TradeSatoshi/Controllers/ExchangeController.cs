using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Web.Helpers;
using TradeSatoshi.Common.Exchange;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Web.Controllers
{
	public class ExchangeController : BaseController
	{
		public ITradeWriter TradeWriter { get; set; }
		public ITradeReader TradeReader { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult TradePair(int tradePairId)
		{
			var model = TradeReader.GetTradePairInfo(tradePairId, User.Id());
			return PartialView("_TradePairPartial", new TradePairExchangeModel
			{
				TradePairId = model.TradePairId,
				Symbol = model.Symbol,
				BaseSymbol = model.BaseSymbol,
				Balance = model.Balance,
				BaseBalance = model.BaseBalance,
				BuyModel = new CreateTradeModel
				{
					TradePairId = model.TradePairId,
					TradeType = TradeType.Buy,
					Symbol = model.Symbol,
					BaseSymbol = model.BaseSymbol,
					Fee = 0.2m,
					MinTrade = 0.00002000m
				},
				SellModel = new CreateTradeModel
				{
					TradePairId = model.TradePairId,
					TradeType = TradeType.Sell,
					Symbol = model.Symbol,
					BaseSymbol = model.BaseSymbol,
					Fee = model.Fee,
					MinTrade = model.MinTrade
				},
			});
		}

		[HttpPost]
		public async Task<ActionResult> CreateTrade(CreateTradeModel model)
		{
			if (!ModelState.IsValid)
				return PartialView("_CreateTradePartial", model);

			model.UserId = User.Id();
			var result = await TradeWriter.CreateTradeAsync(model);
			if (!ModelState.IsWriterResultValid(result))
				return PartialView("_CreateTradePartial", model);

			return PartialView("_CreateTradePartial", model);
		}

		[HttpPost]
		public async Task<ActionResult> CancelTrade(int id, CancelTradeType cancelType)
		{
			await TradeWriter.CancelTradeAsync(new CancelTradeModel
			{
				UserId = User.Id(),
				TradeId = id,
				TradePairId = id,
				CancelType = cancelType
			});

			return JsonSuccess();
		}

		[HttpPost]
		public async Task<ActionResult> GetOrderBook(DataTablesModel param, int tradePairId, TradeType tradeType)
		{
			return DataTable(TradeReader.GetTradePairOrderBookDataTable(param, tradePairId, tradeType));
		}

		[HttpPost]
		public async Task<ActionResult> GetMarketHistory(DataTablesModel param, int tradePairId)
		{
			return DataTable(TradeReader.GetTradePairTradeHistoryDataTable(param, tradePairId));
		}

		[HttpPost]
		public async Task<ActionResult> GetUserOpenOrders(DataTablesModel param, int tradePairId)
		{
			return DataTable(TradeReader.GetTradePairUserOpenOrdersDataTable(param, tradePairId, User.Id()));
		}

		[HttpPost]
		public async Task<ActionResult> GetUserMarketHistory(DataTablesModel param, int tradePairId)
		{
			return DataTable(TradeReader.GetUserTradePairTradeHistoryDataTable(param, tradePairId, User.Id()));
		}
	}
}