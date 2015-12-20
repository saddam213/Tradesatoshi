﻿using System;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Services.NotificationService;
using TradeSatoshi.Common.Vote;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class HomeController : BaseController
	{
		public IDataContext DataContext { get; set; }
		public INotificationService NotificationService { get; set; }
		public IBalanceReader BalanceReader { get; set; }

		public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated)
			{
				var test = BalanceReader.GetBalance(User.Id(), 2);
				var test2 = BalanceReader.GetBalances(User.Id());
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



			return View();
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
			return CloseModal();
		}

		#region Coin Voting

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Voting()
		{
			return View();
		}

		[HttpGet]
		[Authorize]
		public ActionResult SubmitVote()
		{
			return RedirectToAction("Voting");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitVote(int voteItemId)
		{
			return RedirectToAction("Voting");
		}

		[HttpPost]
		[Authorize]
		public ActionResult CreateVoteItem()
		{
			return View(new CreateItemModel());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitVoteItem(CreateItemModel model)
		{
			return RedirectToAction("Voting");
		}

		#endregion
	}




}