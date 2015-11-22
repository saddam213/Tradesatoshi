using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using hbehr.recaptcha;
using System.Data.Entity.Validation;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Helpers;
using System;

namespace TradeSatoshi.Controllers
{
	[Authorize]
	public class AccountController : BaseController
	{
		#region Properties

		public IEmailService EmailService { get; set; }

		#endregion

		#region Login

		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
				return View(model);

			//does user exist
			var user = await UserManager.FindByNameAsync(model.UserName);
			if (user == null)
			{
				ModelState.AddModelError("", this.Resource("Invalid username or password."));
				return View(model);
			}

			//is the users email confirmed
			if (!await UserManager.IsEmailConfirmedAsync(user.Id))
			{
				ModelState.AddModelError("", "An email has been sent to your registered email address with a confirmation link.");
				return View(model);
			}

			// is the user locked out
			if (await UserManager.IsLockedOutAsync(user.Id))
			{
				ModelState.AddModelError("", "Your account is locked.");
				return View(model);
			}

			// is the password correct
			var lockoutLink = Url.Action("LockAccount", "Account", new { username = user.UserName, lockoutToken = await UserManager.GenerateUserTokenAsync("LockAccount", user.Id) }, protocol: Request.Url.Scheme);
			if (await UserManager.CheckPasswordAsync(user, model.Password))
			{
				await UserManager.ResetAccessFailedCountAsync(user.Id);
				await SignInAsync(user, model.RememberMe);
				await EmailService.SendAsync(EmailTemplate.Logon, model.UserName, Request.GetIPAddress(), lockoutLink);
				return RedirectToLocal(returnUrl);
			}
			else
			{
				await UserManager.AccessFailedAsync(user.Id);
				if (await UserManager.IsLockedOutAsync(user.Id))
				{
					await EmailService.SendAsync(EmailTemplate.PasswordLockout, model.UserName, Request.GetIPAddress());
					return View(model);
				}
				ModelState.AddModelError("", string.Format("Email or password was invalid.", UserManager.MaxFailedAccessAttemptsBeforeLockout - user.AccessFailedCount));
				await EmailService.SendAsync(EmailTemplate.FailedLogon, model.UserName, Request.GetIPAddress(), lockoutLink);
				ViewBag.Lockout = lockoutLink;
				return View(model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		#endregion

		#region Register

		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				string userResponse = HttpContext.Request.Params["g-recaptcha-response"];
				bool validCaptcha = ReCaptcha.ValidateCaptcha(userResponse);
				if (!validCaptcha)
				{
					ModelState.AddModelError("", this.Resource("Invalid reCaptcha"));
					return View(model);
				}

				var user = new ApplicationUser()
				{
					UserName = model.UserName,
					Email = model.EmailAddress
				};
				user.Profile = new UserProfile { Id = user.Id };
				user.Settings = new UserSettings { Id = user.Id };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					string confirmationToken = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = Url.Action("RegisterConfirmEmail", "Account", new { username = user.UserName, confirmationToken = confirmationToken }, protocol: Request.Url.Scheme);
					if (await EmailService.SendAsync(EmailTemplate.Registration, user.UserName, Request.GetIPAddress()))
					{
						return ViewMessage(new ViewMessageModel(ViewMessageType.Info, "Confirmation Email Sent.", string.Format("An email has been sent to {0}, please click the activation link in the email to complete your registration process. <br /><strong>DEBUG ACTIVATION LINK: </strong> <a href='{1}'>Confirm Email</a>", user.Email, callbackUrl)));
					}

					ModelState.AddModelError("", "Failed to send registration confirmation email, if problem persists please contact Support.");
					return RedirectToAction("Index", "Home");
				}
				else
				{
					AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> RegisterConfirmEmail(string username, string confirmationToken)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(confirmationToken))
				return Unauthorized();

			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return Unauthorized();

			var result = await UserManager.ConfirmEmailAsync(user.Id, confirmationToken);
			if (result.Succeeded)
			{
				return ViewMessage(new ViewMessageModel(ViewMessageType.Success, "Email Succesfully Confirmed.", string.Format("Thank you for confirming your email address, your registration is now complete, please <a href='{0}'>click here to login</a>.", Url.Action("Login"))));
			}

			return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Invalid Activation Link.", ""));
		}

		#endregion

		#region LockAccount

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> LockAccount(string username, string lockoutToken)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(lockoutToken))
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Invalid Lockout Token", "The secure lockout token is invalid or has expired."));

			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return Unauthorized();

			if (!await UserManager.VerifyUserTokenAsync(user.Id, "LockAccount", lockoutToken))
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Invalid Lockout Token", "The secure lockout token is invalid or has expired."));

			if (await UserManager.IsLockedOutAsync(user.Id))
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Account Locked", "Your account has already been locked.."));

			await UserManager.SetLockoutEndDateAsync(user.Id, DateTime.UtcNow.AddYears(1));
			await UserManager.UpdateSecurityStampAsync(user.Id);
			await EmailService.SendAsync(EmailTemplate.UserLockout, user.UserName, Request.GetIPAddress());
			return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, "Account Lockdown!", "Your account has been locked at your request,"));
		}

		#endregion

		#region Manage

		[HttpGet]
		public ActionResult Manage(ManageMessageId? message)
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? this.Resource("Your password has been changed.")
				: message == ManageMessageId.Error ? this.Resource("An error has occurred.")
				: "";
			ViewBag.ReturnUrl = Url.Action("Manage");
			return View(new ManageUserViewModel
			{
				Profile = new UserProfileModel
				{
					BirthDate = user.Profile.BirthDate,
					City = user.Profile.City,
					Country = user.Profile.Country,
					Address = user.Profile.Address,
					FirstName = user.Profile.FirstName,
					LastName = user.Profile.LastName,
					PostCode = user.Profile.PostCode,
					State = user.Profile.State,
					CanUpdate = user.Profile.CanUpdate()
				}
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Manage(ManageUserViewModel model)
		{
			ViewBag.ReturnUrl = Url.Action("Manage");
			if (ModelState.IsValid)
			{
				IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
				if (result.Succeeded)
				{
					return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
				}
				else
				{
					AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UserProfile(UserProfileModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("Manage", new ManageUserViewModel { Profile = model });
			}

			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user.Profile.CanUpdate())
			{
				user.Profile.Address = model.Address;
				user.Profile.BirthDate = model.BirthDate;
				user.Profile.City = model.City;
				user.Profile.Country = model.Country;
				user.Profile.FirstName = model.FirstName;
				user.Profile.LastName = model.LastName;
				user.Profile.PostCode = model.PostCode;
				user.Profile.State = model.State;

				await UserManager.UpdateAsync(user);
				model.CanUpdate = user.Profile.CanUpdate();
			}

			return View("Manage", new ManageUserViewModel { Profile = model });
		}

		#endregion

		#region Helpers

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private async Task SignInAsync(ApplicationUser user, bool isPersistent)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
			var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
			AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			Error
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		#endregion
	}
}