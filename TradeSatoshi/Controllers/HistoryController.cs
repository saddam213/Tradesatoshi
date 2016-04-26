using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Deposit;
using TradeSatoshi.Common.History;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class HistoryController : BaseController
	{
		public IBalanceReader BalanceReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IDepositReader DepositReader { get; set; }
		public IWithdrawReader WithdrawReader { get; set; }
		public ITransferReader TransferReader { get; set; }
		public ITradeReader TradeReader { get; set; }

		public async Task<ActionResult> Index(string area = "Deposit", string coin = null)
		{
			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			return View(new HistoryViewModel
			{
				Currency = coin,
				Section = area,
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances)
			});
		}

		#region Deposit

		[HttpGet]
		public ActionResult Deposit(string search)
		{
			ViewBag.Search = search;
			return PartialView("_DepositPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetDeposits(DataTablesModel param)
		{
			return DataTable(await DepositReader.GetUserDepositDataTable(param, User.Id()));
		}

		#endregion

		#region Withdraw

		[HttpGet]
		public ActionResult Withdraw(string search)
		{
			ViewBag.Search = search;
			return PartialView("_WithdrawPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetWithdraws(DataTablesModel param)
		{
			return DataTable(await WithdrawReader.GetUserWithdrawDataTable(param, User.Id()));
		}

		#endregion

		#region Transfer

		[HttpGet]
		public ActionResult Transfers(string search)
		{
			ViewBag.Search = search;
			return PartialView("_TransferPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetTransfers(DataTablesModel param)
		{
			return DataTable(await TransferReader.GetUserTransferDataTable(param, User.Id()));
		}

		#endregion

		#region Trade

		[HttpGet]
		public ActionResult Trades(string search)
		{
			ViewBag.Search = search;
			return PartialView("_TradePartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetTrades(DataTablesModel param)
		{
			return DataTable(await TradeReader.GetUserTradeHistoryDataTable(param, User.Id()));
		}

		#endregion
	}
}