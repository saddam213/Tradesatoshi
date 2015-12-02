using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Core.Admin;
using TradeSatoshi.Models;
using TradeSatoshi.Models.Admin;

namespace TradeSatoshi.Controllers
{
	//[Authorize(Roles = "Admin")]
	public class AdminController : BaseController
	{
		public IUserReader UserReader { get; set; }
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
			var userupdateModel = await UserReader.GetUserUpdateAsync(userId);
			if (userupdateModel == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!", "Unable to find user information."));

			return View("UpdateUserModal", userupdateModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateUser(UpdateUserModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateUserModal", model);

			var result = await UserWriter.UpdateUserAsync(model);
			if (result.HasError)
			{
				ModelState.AddModelError("", result.Error);
				return View("UpdateUserModal", model);
			}

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
			return DataTable(UserReader.GetLogonDataTable(param));
		}

		[HttpPost]
		public ActionResult GetSecurityRoles(DataTablesModel param, SecurityRole securityRole)
		{
			return DataTable(UserReader.GetRolesDataTable(param, securityRole));
		}

		[HttpGet]
		public ActionResult AddSecurityRole(string user)
		{
			return View("UpdateRoleModal", new UpdateUserRoleModel
			{
				UserName = user,
				ButtonName = "Add Role",
				ActionMethod = "AddSecurityRole",
				Description = string.Format("Add a security role to '{0}'", user)
			});
		}

		[HttpPost]
		public async Task<ActionResult> AddSecurityRole(UserRoleModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateRoleModal", model);

			var result = await UserWriter.AddUserRoleAsync(model);
			if (result.HasError)
			{
				ModelState.AddModelError("", result.Error);
				return View("UpdateRoleModal", new UpdateUserRoleModel
				{
					UserName = model.UserName,
					ButtonName = "Add Role",
					ActionMethod = "AddSecurityRole",
					Description = string.Format("Add a security role to '{0}'", model.UserName)
				});
			}

			return CloseModal();
		}

		[HttpGet]
		public ActionResult RemoveSecurityRole(string user)
		{
			return View("UpdateRoleModal", new UpdateUserRoleModel
			{
				UserName = user,
				ButtonName = "Remove Role",
				ActionMethod = "RemoveSecurityRole",
				Description = string.Format("Remove a security role from '{0}'", user)
			});
		}

		[HttpPost]
		public async Task<ActionResult> RemoveSecurityRole(UserRoleModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateRoleModal", model);

			var result = await UserWriter.RemoveUserRoleAsync(model);
			if (result.HasError)
			{
				ModelState.AddModelError("", result.Error);
				return View("UpdateRoleModal", new UpdateUserRoleModel
				{
					UserName = model.UserName,
					ButtonName = "Remove Role",
					ActionMethod = "RemoveSecurityRole",
					Description = string.Format("Remove a security role from '{0}'", model.UserName)
				});
			}

			return CloseModal();
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