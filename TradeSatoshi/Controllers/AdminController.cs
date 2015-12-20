using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Deposit;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Core.Admin;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	[AuthorizeSecurityRole(SecurityRole.Administrator)]
	public class AdminController : BaseController
	{
		public IUserReader UserReader { get; set; }
		public IUserWriter UserWriter { get; set; }
		public IDepositReader DepositReader { get; set; }
		public IWithdrawReader WithdrawReader { get; set; }
		public ITradeReader TradeReader { get; set; }

		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		#region Status

		[HttpGet]
		public ActionResult Status()
		{
			return PartialView("_StatusPartial");
		}

		#endregion

		#region Accounts

		[HttpGet]
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

		[HttpGet]
		public ActionResult Security()
		{
			return PartialView("_SecurityPartial", new AdminSecurityModel());
		}

		[HttpPost]
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

		[HttpGet]
		public ActionResult Deposits()
		{
			return PartialView("_DepositsPartial");
		}

		[HttpPost]
		public ActionResult GetDeposits(DataTablesModel param)
		{
			return DataTable(DepositReader.GetDepositDataTable(param));
		}


		[HttpGet]
		public ActionResult Withdrawals()
		{
			return PartialView("_WithdrawalsPartial");
		}

		[HttpPost]
		public ActionResult GetWithdrawals(DataTablesModel param)
		{
			return DataTable(WithdrawReader.GetWithdrawDataTable(param));
		}

		#endregion

		#region Trades

		[HttpGet]
		public ActionResult Trades()
		{
			return PartialView("_TradesPartial");
		}

		[HttpPost]
		public ActionResult GetTrades(DataTablesModel param)
		{
			return DataTable(TradeReader.GetTradeDataTable(param));
		}


		[HttpGet]
		public ActionResult TradeHistory()
		{
			return PartialView("_TradeHistoryPartial");
		}

		[HttpPost]
		public ActionResult GetTradeHistory(DataTablesModel param)
		{
			return DataTable(TradeReader.GetTradeHistoryDataTable(param));
		}

		#endregion

		#region Statistics

		[HttpGet]
		public ActionResult Statistics()
		{
			return PartialView("_StatisticsPartial");
		}

		#endregion

		#region Voting

		[HttpGet]
		public ActionResult Voting()
		{
			return PartialView("_VotingPartial");
		}

		#endregion
	}
}