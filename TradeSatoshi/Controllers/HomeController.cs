using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Exchange;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Services.NotificationService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.Vote;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class HomeController : BaseController
	{
		public IDataContext DataContext { get; set; }
		public INotificationService NotificationService { get; set; }
		public IBalanceReader BalanceReader { get; set; }
		public ITradeWriter TradeWriter { get; set; }
		public ITradeReader TradeReader { get; set; }
	
		public ITradePairReader TradePairReader { get; set; }


		public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated)
			{
				//var test = BalanceReader.GetBalance(User.Id(), 2);
				//var test2 = BalanceReader.GetBalances(User.Id());
			}

			using (var context = DataContext.CreateContext())
			{
				try
				{
					//var users = context.Users.FirstOrDefault(x => x.UserName == "test2");
					//users.EmailConfirmed = true;
					//context.UserTwoFactor.RemoveRange(users.TwoFactor);
					//context.SaveChanges();
				}
				catch (DbEntityValidationException ex)
				{


				}
			}

			return View();
		}


		public async Task<ActionResult> Contact()
		{



			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			return View(new ExchangeModel
			{
				TradePair = "DOT_BTC",
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances)
			});
		}

		public ActionResult Terms()
		{
			return View();
		}

		public ActionResult Test()
		{
			foreach (NotificationType item in Enum.GetValues(typeof(NotificationType)))
			{
				NotificationService.SendNotification(new Notification
				{
					Message = "Test message",
					Title = "Test title",
					Type = item
				});

				NotificationService.SendDataNotificationAsync(new DataNotification
 				{
					ElementName = "#test",
					ElementValue = "Hello World!"
				});

				if (User.Identity.IsAuthenticated)
				{
					NotificationService.SendUserNotification(new UserNotification
					{
						Message = "Test message",
						Title = "Test title",
						Type = item,
						UserId = User.Id()
					});
				}
			}


			return Index();
		}

		public ActionResult Test2()
		{
			return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Danger", "Danger! Will Robertson, Danger!"));
		}

		[HttpPost]
		public ActionResult Test3()
		{
			return CloseModalRedirect(Url.Action("Voting", "Home"));
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
		public ActionResult GetOrderBook(DataTablesModel param, int tradePairId, TradeType tradeType)
		{
			return DataTable(TradeReader.GetTradePairOrderBookDataTable(param, tradePairId, tradeType));
		}

		[HttpPost]
		public ActionResult GetMarketHistory(DataTablesModel param, int tradePairId)
		{
			return DataTable(TradeReader.GetTradePairTradeHistoryDataTable(param, tradePairId));
		}

		[HttpGet]
		public async Task<ActionResult> GetTradePairChart(int tradePairId)
		{
			return Json(await TradeReader.GetTradePairChart(tradePairId), JsonRequestBehavior.AllowGet);
		}
	}
}