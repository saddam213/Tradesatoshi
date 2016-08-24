using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Deposit;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Repositories.Admin;
using TradeSatoshi.Common.Repositories.Email;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Common.Support;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Vote;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Core.Admin;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1, SecurityRole.Moderator2)]
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
		public IEmailTemplateReader EmailTemplateReader { get; set; }
		public IEmailTemplateWriter EmailTemplateWriter { get; set; }
		public IEmailService EmailService { get; set; }
		public IWithdrawWriter WithdrawWriter { get; set; }


		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		#region Status

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public async Task<ActionResult> Status()
		{
			return PartialView("_StatusPartial", await SiteStatusReader.GetSiteStatus());
		}

		#endregion

		#region Currency

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult Currency()
		{
			return PartialView("_CurrencyPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetCurrencies(DataTablesModel param)
		{
			return DataTable(await CurrencyReader.GetCurrencyDataTable(param));
		}

		#endregion

		#region TradePair

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult TradePair()
		{
			return PartialView("_TradePairPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetTradePairs(DataTablesModel param)
		{
			return DataTable(await TradePairReader.GetTradePairDataTable(param));
		}

		#endregion

		#region Accounts

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public ActionResult Accounts()
		{
			return PartialView("_AccountsPartial", new AdminAccountsModel());
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public async Task<ActionResult> GetUsers(DataTablesModel param)
		{
			return DataTable(await UserReader.GetUserDataTable(param));
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public async Task<ActionResult> UpdateUser(string userId)
		{
			var userupdateModel = await UserReader.GetUserUpdate(userId);
			if (userupdateModel == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!", "Unable to find user information."));

			return View("UpdateUserModal", userupdateModel);
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
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
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult Security()
		{
			return PartialView("_SecurityPartial", new AdminSecurityModel());
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetLogons(DataTablesModel param)
		{
			return DataTable(await UserReader.GetLogonDataTable(param));
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetSecurityRoles(DataTablesModel param, SecurityRole securityRole)
		{
			return DataTable(await UserReader.GetRolesDataTable(param, securityRole));
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
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
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
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
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
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
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
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
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult Deposits()
		{
			return PartialView("_DepositsPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetDeposits(DataTablesModel param)
		{
			return DataTable(await DepositReader.GetDepositDataTable(param));
		}


		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult Withdrawals()
		{
			return PartialView("_WithdrawalsPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetWithdrawals(DataTablesModel param)
		{
			return DataTable(await WithdrawReader.GetWithdrawDataTable(param));
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> CancelWithdraw(string username, int withdrawId)
		{
			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return Unauthorized();

			var result = await WithdrawWriter.CancelWithdraw(user.Id, withdrawId);
			if (result.HasErrors)
				return JsonError(result.FirstError, "Cancelation Failed");

			return JsonSuccess(string.Format("Successfully canceled withdraw #{0}.", withdrawId), "Success");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> ResendConfirmationEmail(string username, int withdrawId)
		{
			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return Unauthorized();

			var confirmToken = await WithdrawReader.GetWithdrawalToken(user.Id, withdrawId);
			if (string.IsNullOrEmpty(confirmToken))
				return JsonError($"Withdrawal #{withdrawId} not found", "Error");

			var cancelWithdrawToken = await UserManager.GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawCancel, user.Id);
			var confirmlink = Url.Action("ConfirmWithdraw", "Withdraw", new { username = user.UserName, secureToken = confirmToken, withdrawid = withdrawId }, protocol: Request.Url.Scheme);
			var cancellink = Url.Action("CancelWithdraw", "Withdraw", new { username = user.UserName, secureToken = cancelWithdrawToken, withdrawid = withdrawId }, protocol: Request.Url.Scheme);
			var result = await EmailService.Send(EmailType.WithdrawConfirmation, user, "Support", new EmailParam("[CONFIRMLINK]", confirmlink), new EmailParam("[CANCELLINK]", cancellink));
			if (!result)
				return JsonError($"Failed to send confirmation email", "Error");

			return JsonSuccess($"Successfully sent confirmation email to {user.Email}", "Success");
		}


		#endregion

		#region Trades

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult Trades()
		{
			return PartialView("_TradesPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetTrades(DataTablesModel param)
		{
			return DataTable(await TradeReader.GetTradeDataTable(param));
		}


		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult TradeHistory()
		{
			return PartialView("_TradeHistoryPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetTradeHistory(DataTablesModel param)
		{
			return DataTable(await TradeReader.GetTradeHistoryDataTable(param));
		}

		#endregion

		#region Transfers

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult Transfers()
		{
			return PartialView("_TransferPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetTransfers(DataTablesModel param)
		{
			return DataTable(await TransferReader.GetTransferDataTable(param));
		}

		#endregion

		#region Support

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1, SecurityRole.Moderator2)]
		public ActionResult Support()
		{
			return PartialView("_SupportPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1, SecurityRole.Moderator2)]
		public async Task<ActionResult> GetSupportTickets(DataTablesModel param)
		{
			return DataTable(await SupportReader.AdminGetSupportTicketDataTable(param));
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1, SecurityRole.Moderator2)]
		public async Task<ActionResult> GetSupportRequests(DataTablesModel param)
		{
			return DataTable(await SupportReader.AdminGetSupportRequestDataTable(param));
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1, SecurityRole.Moderator2)]
		public async Task<ActionResult> GetSupportCategory(DataTablesModel param)
		{
			return DataTable(await SupportReader.AdminGetSupportCategoryDataTable(param));
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1, SecurityRole.Moderator2)]
		public async Task<ActionResult> GetSupportFaq(DataTablesModel param)
		{
			return DataTable(await SupportReader.AdminGetSupportFaqDataTable(param));
		}

		#endregion

		#region Voting

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public ActionResult Voting()
		{
			return PartialView("_VotingPartial");
		}

		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public async Task<ActionResult> GetVoteItems(DataTablesModel param)
		{
			return DataTable(await VoteReader.AdminGetVoteDataTable(param));
		}

		#endregion

		#region EmailTemplate

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> EmailTemplate()
		{
			var model = await EmailTemplateReader.GetEmailTemplates();
			return PartialView("_EmailTemplatePartial", model);
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> GetEmailTemplate(EmailType emailType)
		{
			var model = await EmailTemplateReader.GetEmailTemplate(emailType);
			return PartialView("_EmailTemplateUpdatePartial", model);
		}


		[HttpPost]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> UpdateEmailTemplate(EmailTemplateModel model)
		{
			if (!ModelState.IsValid)
				return PartialView("_EmailTemplateUpdatePartial", model);

			var result = await EmailTemplateWriter.UpdateEmailTemplate(model);
			if(!ModelState.IsWriterResultValid(result)) 
					return PartialView("_EmailTemplateUpdatePartial", model);

			ViewBag.Success = result.Message;
			return PartialView("_EmailTemplateUpdatePartial", model);
		}

		#endregion
	}
}