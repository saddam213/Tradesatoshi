using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Helpers;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Data;
using System.Data.Entity;
using TradeSatoshi.Models;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Common.Data.Entities;

namespace TradeSatoshi.Controllers
{
	public class WithdrawController : BaseController
	{
		#region Properties

		public IEmailService EmailService { get; set; }
		public IWithdrawReader WithdrawReader { get; set; }
		public IWithdrawWriter WithdrawWriter { get; set; }

		#endregion

		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<ActionResult> Create(int currencyId)
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			var model = await WithdrawReader.GetCreateWithdrawAsync(User.Id(), currencyId);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Invalid Request", "An unknown error occured."));

			model.TwoFactorComponentType = TwoFactorComponentType.Withdraw;
			model.TwoFactorType = await UserManager.GetUserTwoFactorTypeAsync(user.Id, TwoFactorComponentType.Withdraw);

			return View("CreateWithdrawModal", model);
		}

		[HttpPost]
		public async Task<ActionResult> Create(CreateWithdrawModel model)
		{
			if (model.TwoFactorType == TwoFactorType.None)
			{
				// If there is no tfa remove the Data field validation.
				ModelState.Remove("Data");
			}

			if (!ModelState.IsValid)
				return View("CreateWithdrawModal", model);

			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			// Verify TwoFactor code.
			if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponentType.Withdraw, user.Id, model.Data))
			{
				ModelState.AddModelError("Data", "Invalid TwoFactor code.");
				return View("CreateWithdrawModal", model);
			}

			// Create withdraw
			var twoFactortoken = await UserManager.GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawConfirm, user.Id);
			model.ConfirmationToken = twoFactortoken;
			var result = await WithdrawWriter.CreateWithdrawAsync(user.Id, model);
			if (result.HasError)
			{
				ModelState.AddModelError("", result.Error);
				return View("CreateWithdrawModal", model);
			}

			int withdrawId = result.Data;

			// Send confirmation email
			await SendConfirmationEmail(user, twoFactortoken, withdrawId, model);

			return ViewMessageModal(new ViewMessageModel(ViewMessageType.Success, "Withdraw Success", "Your withdraw request has been sucessfully submitted, A confirmation email has been sent to your registered email address."));
		}

		[AllowAnonymous]
		public async Task<ActionResult> ConfirmWithdraw(string username, string secureToken, int withdrawid)
		{
			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return Unauthorized();

			if (!await UserManager.VerifyUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawConfirm, user.Id, secureToken))
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Invalid Security Token", string.Format("Security token for withdraw #{0} is invalid or has expired, You can send a new one from the withdrawal section in your account page.", withdrawid)));

			var result = await WithdrawWriter.ConfirmWithdrawAsync(user.Id, withdrawid);
			if(result.HasError)
			{
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Withdrawal Confirm Failed", string.Format("Failed to confirm withdraw #{0}.", withdrawid)));
			}

			return ViewMessage(new ViewMessageModel(ViewMessageType.Success, "Withdrawal Confirmed", string.Format("Successfully confirmed withdraw #{0}.", withdrawid)));
		}

		[AllowAnonymous]
		public async Task<ActionResult> CancelWithdraw(string username, string secureToken, int withdrawid)
		{
			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return Unauthorized();

			if (!await UserManager.VerifyUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawCancel, user.Id, secureToken))
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Invalid Security Token", string.Format("Security token for withdraw #{0} is invalid or has expired, You can send a new one from the withdrawal section in your account page.", withdrawid)));

			var result = await WithdrawWriter.CancelWithdrawAsync(user.Id, withdrawid);
			if (result.HasError)
			{
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Withdrawal Cancel Failed", string.Format("Failed to cancel withdraw #{0}.", withdrawid)));
			}

			return ViewMessage(new ViewMessageModel(ViewMessageType.Success, "Withdrawal Canceled", string.Format("Successfully canceled withdraw #{0}.", withdrawid)));
		}

		#region Helpers

		private async Task<bool> SendConfirmationEmail(ApplicationUser user, string confirmToken, int withdrawId, CreateWithdrawModel model)
		{
			var cancelWithdrawToken = await UserManager.GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawCancel, user.Id);
			var confirmlink = Url.Action("ConfirmWithdraw", "Withdraw", new { username = user.UserName, secureToken = confirmToken, withdrawid = withdrawId }, protocol: Request.Url.Scheme);
			var cancellink = Url.Action("CancelWithdraw", "Withdraw", new { username = user.UserName, secureToken = cancelWithdrawToken, withdrawid = withdrawId }, protocol: Request.Url.Scheme);
			return await EmailService.SendAsync(EmailType.WithdrawConfirmation, user, Request.GetIPAddress(), confirmlink, cancellink);
		}

		#endregion
	}
}