using System.Web.Mvc;
using System.Threading.Tasks;
using TradeSatoshi.Models;
using TradeSatoshi.Data.DataContext;
using System.Linq;
using System.Collections.Generic;
using TradeSatoshi.Data.Entities;
using System;
using System.Data.Entity.Validation;
using TradeSatoshi.Models.Vote;

namespace TradeSatoshi.Controllers
{
	public class HomeController : BaseController
	{
		public IDataContext DataContext { get; set; }

		public ActionResult Index()
		{
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

		public ActionResult Contact()
		{
			return View();
		}

		public ActionResult Terms()
		{
			return View();
		}

		public ActionResult Test()
		{
			return View();
		}

		public ActionResult Test2()
		{
			return ViewMessageModal(new ViewMessageModel( ViewMessageType.Danger, "Danger", "Danger! Will Robertson, Danger!"));
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