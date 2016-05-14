using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Deposit;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Repositories.Admin;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Support;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Vote;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Core.Admin;
using TradeSatoshi.Web.Attributes;
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
		public ITransferReader TransferReader { get; set; }
		public ISupportReader SupportReader { get; set; }
		public IVoteReader VoteReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public ISiteStatusReader SiteStatusReader { get; set; }

		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		#region Status

		[HttpGet]
		public async Task<ActionResult> Status()
		{
			return PartialView("_StatusPartial", await SiteStatusReader.GetSiteStatus());
		}

		#endregion

		#region Currency

		[HttpGet]
		public ActionResult Currency()
		{
			return PartialView("_CurrencyPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetCurrencies(DataTablesModel param)
		{
			return DataTable(await CurrencyReader.GetCurrencyDataTable(param));
		}

		#endregion

		#region TradePair

		[HttpGet]
		public ActionResult TradePair()
		{
			return PartialView("_TradePairPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetTradePairs(DataTablesModel param)
		{
			return DataTable(await TradePairReader.GetTradePairDataTable(param));
		}

		#endregion

		#region Accounts

		[HttpGet]
		public ActionResult Accounts()
		{
			return PartialView("_AccountsPartial", new AdminAccountsModel());
		}

		[HttpPost]
		public async Task<ActionResult> GetUsers(DataTablesModel param)
		{
			return DataTable(await UserReader.GetUserDataTable(param));
		}

		[HttpGet]
		public async Task<ActionResult> UpdateUser(string userId)
		{
			var userupdateModel = await UserReader.GetUserUpdate(userId);
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

			var result = await UserWriter.UpdateUser(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateUserModal", model);

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
		public async Task<ActionResult> GetLogons(DataTablesModel param)
		{
			return DataTable(await UserReader.GetLogonDataTable(param));
		}

		[HttpPost]
		public async Task<ActionResult> GetSecurityRoles(DataTablesModel param, SecurityRole securityRole)
		{
			return DataTable(await UserReader.GetRolesDataTable(param, securityRole));
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

			var result = await UserWriter.AddUserRole(model);
			if (!ModelState.IsWriterResultValid(result))
			{
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

			var result = await UserWriter.RemoveUserRole(model);
			if (!ModelState.IsWriterResultValid(result))
			{
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
		public async Task<ActionResult> GetDeposits(DataTablesModel param)
		{
			return DataTable(await DepositReader.GetDepositDataTable(param));
		}


		[HttpGet]
		public ActionResult Withdrawals()
		{
			return PartialView("_WithdrawalsPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetWithdrawals(DataTablesModel param)
		{
			return DataTable(await WithdrawReader.GetWithdrawDataTable(param));
		}

		#endregion

		#region Trades

		[HttpGet]
		public ActionResult Trades()
		{
			return PartialView("_TradesPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetTrades(DataTablesModel param)
		{
			return DataTable(await TradeReader.GetTradeDataTable(param));
		}


		[HttpGet]
		public ActionResult TradeHistory()
		{
			return PartialView("_TradeHistoryPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetTradeHistory(DataTablesModel param)
		{
			return DataTable(await TradeReader.GetTradeHistoryDataTable(param));
		}

		#endregion

		#region Transfers

		[HttpGet]
		public ActionResult Transfers()
		{
			return PartialView("_TransferPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetTransfers(DataTablesModel param)
		{
			return DataTable(await TransferReader.GetTransferDataTable(param));
		}

		#endregion

		#region Support

		[HttpGet]
		public ActionResult Support()
		{
			return PartialView("_SupportPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetSupportTickets(DataTablesModel param)
		{
			return DataTable(await SupportReader.AdminGetSupportTicketDataTable(param));
		}

		[HttpPost]
		public async Task<ActionResult> GetSupportRequests(DataTablesModel param)
		{
			return DataTable(await SupportReader.AdminGetSupportRequestDataTable(param));
		}

		[HttpPost]
		public async Task<ActionResult> GetSupportCategory(DataTablesModel param)
		{
			return DataTable(await SupportReader.AdminGetSupportCategoryDataTable(param));
		}

		[HttpPost]
		public async Task<ActionResult> GetSupportFaq(DataTablesModel param)
		{
			return DataTable(await SupportReader.AdminGetSupportFaqDataTable(param));
		}

		#endregion

		#region Voting

		[HttpGet]
		public ActionResult Voting()
		{
			return PartialView("_VotingPartial");
		}

		[HttpPost]
		public async Task<ActionResult> GetVoteItems(DataTablesModel param)
		{
			return DataTable(await VoteReader.AdminGetVoteDataTable(param));
		}

		#endregion
	}
}