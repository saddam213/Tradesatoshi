using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Core.Admin;
using TradeSatoshi.Models;
using TradeSatoshi.Models.Admin;

namespace TradeSatoshi.Controllers
{
	//[Authorize(Roles = "Admin")]
	public class AdminController : BaseController
	{
		public IUserReader UserReader { get; set; }
		public ILogonReader LogonReader { get; set; }
		public IUserWriter UserWriter { get; set; }

		//[Authorize(Roles = "Admin")]
		public ActionResult Index()
		{
			return View();
		}

		#region Status

		//[Authorize(Roles = "Admin")]
		public ActionResult Status()
		{
			return PartialView("_StatusPartial");
		}

		#endregion

		#region Accounts

		//[Authorize(Roles = "Admin")]
		public ActionResult Accounts()
		{
			return PartialView("_AccountsPartial", new AdminAccountsModel());
		}

		[HttpPost]
		public ActionResult GetUsers(DataTablesModel param)
		{
			return DataTable(UserReader.GetUserDataTable(param));
		}

		[HttpGet]
		public async Task<ActionResult> UpdateUser(string userId)
		{
			var userupdateModel =  await UserReader.GetUserUpdateAsync(userId);
			if (userupdateModel == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!", "Unable to find user information."));

			return View("UpdateUserModal", userupdateModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateUser(UpdateUserModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await UserWriter.UpdateUserAsync(model);
			if(!result)
				return View(model);

			return CloseModal();
		}

		#endregion

		#region Security

		//[Authorize(Roles = "Admin")]
		public ActionResult Security()
		{
			return PartialView("_SecurityPartial", new AdminSecurityModel());
		}

		[HttpPost]
		//[Authorize(Roles = "Admin")]
		public ActionResult GetLogons(DataTablesModel param)
		{
			return DataTable(LogonReader.GetLogonDataTable(param));
		}

		#endregion

		#region Deposits/Withdrawals

		//[Authorize(Roles = "Admin")]
		public ActionResult Deposits()
		{
			return PartialView("_DepositsPartial");
		}

		//[Authorize(Roles = "Admin")]
		public ActionResult Withdrawals()
		{
			return PartialView("_WithdrawalsPartial");
		}

		#endregion

		#region Statistics

		//[Authorize(Roles = "Admin")]
		public ActionResult Statistics()
		{
			return PartialView("_StatisticsPartial");
		}

		#endregion

		#region Voting

		//[Authorize(Roles = "Admin")]
		public ActionResult Voting()
		{
			return PartialView("_VotingPartial");
		}

		#endregion
	}
}