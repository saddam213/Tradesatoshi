using hbehr.recaptcha;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Account;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Data;
using TradeSatoshi.Entity;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
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
			if (await UserManager.CheckPasswordAsync(user, model.Password))
			{
				await UserManager.ResetAccessFailedCountAsync(user.Id);

				// Does the user have TFA
				var loginTwoFactor = await UserManager.GetUserTwoFactorTypeAsync(user.Id, TwoFactorComponentType.Login);
				if (loginTwoFactor != TwoFactorType.None)
				{
					SetTwoFactorLoginAuthCookie(user.Id);
					if (loginTwoFactor == TwoFactorType.EmailCode)
					{
						var emailCode = await UserManager.GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, user.Id);
						await EmailService.SendAsync(EmailType.TwoFactorLogin, user, Request.GetIPAddress(), emailCode);
					}

					// Redirect to code verification page
					return RedirectToAction("VerifyTwoFactor");
				}

				// Sign them in
				await SignInAsync(user);
				return RedirectToLocal(returnUrl);
			}
			else
			{
				await UserManager.AccessFailedAsync(user.Id);
				if (await UserManager.IsLockedOutAsync(user.Id))
				{
					await UserManager.AddUserLogon(user, Request.GetIPAddress(), false);
					await EmailService.SendAsync(EmailType.PasswordLockout, user, Request.GetIPAddress());
					return View(model);
				}
				ModelState.AddModelError("", string.Format("Email or password was invalid.", UserManager.MaxFailedAccessAttemptsBeforeLockout - user.AccessFailedCount));
				await UserManager.AddUserLogon(user, Request.GetIPAddress(), false);
				await EmailService.SendAsync(EmailType.FailedLogon, user, Request.GetIPAddress(), GetLockoutLink(user));
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
					Email = model.EmailAddress,
					IsEnabled = true,
					IsTradeEnabled = true,
					IsWithdrawEnabled = true
				};
				user.Profile = new UserProfile { Id = user.Id };
				user.Settings = new UserSettings { Id = user.Id };

				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await UserManager.AddToRoleAsync(user.Id, SecurityRoles.Standard);
					string confirmationToken = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = Url.Action("RegisterConfirmEmail", "Account", new { username = user.UserName, confirmationToken = confirmationToken }, protocol: Request.Url.Scheme);
					if (await EmailService.SendAsync(EmailType.Registration, user, Request.GetIPAddress(), callbackUrl))
					{
						return ViewMessage(new ViewMessageModel(ViewMessageType.Info, "Confirmation Email Sent.", string.Format("An email has been sent to {0}, please click the activation link in the email to complete your registration process. <br /><br /><strong>DEBUG ACTIVATION LINK: </strong> <a href='{1}'>Confirm Email</a>", user.Email, callbackUrl)));
					}

					ModelState.AddModelError("", "Failed to send registration confirmation email, if problem persists please contact Support.");
					return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Email Send Failed.", string.Format("Failed to send email to {0}, please contact <a href='/Support'>Support</a>. <br /><br /><strong>DEBUG ACTIVATION LINK: </strong> <a href='{1}'>Confirm Email</a>", user.Email, callbackUrl)));
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

		#region Password

		/// <summary>
		/// GET: Forgot password view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult PasswordForgot()
		{
			return View();
		}

		/// <summary>
		/// POST: Forgot password. sends a secure token link to users email
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PasswordForgot(PasswordForgotModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var message = new ViewMessageModel(ViewMessageType.Info, "Reset Email Sent", "An email has been sent to your registered email address with password reset instructions.");
			var user = await UserManager.FindByEmailAsync(model.Email);
			if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)) || !user.IsEnabled)
			{
				// Don't reveal that the user does not exist or is not confirmed
				return ViewMessage(message);
			}

			var resetPasswordToken = Url.Action("PasswordReset", "Account", new { secureToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id) }, protocol: Request.Url.Scheme);
			await EmailService.SendAsync(EmailType.PasswordReset, user, Request.GetIPAddress(), resetPasswordToken);
			return ViewMessage(message);
		}

		/// <summary>
		/// GET: Resets the password with the code from the link sent to the user.
		/// </summary>
		/// <param name="code">The code.</param>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult PasswordReset(string secureToken)
		{
			return View("PasswordReset", new PasswordResetModel { SecureToken = secureToken });
		}

		/// <summary>
		/// Resets the users password.
		/// </summary>
		/// <param name="model">The model.</param>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PasswordReset(PasswordResetModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			
			var user = await UserManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return ViewMessage(new ViewMessageModel(ViewMessageType.Success, "Password Reset.", "Your password has been reset"));
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.SecureToken, model.Password);
			if (result.Succeeded)
			{
				return ViewMessage(new ViewMessageModel(ViewMessageType.Success, "Password Reset.", "Your password has been reset"));
			}

			return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Password Reset Failed.", "Falied to reset password."));
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

			if (!await UserManager.VerifyUserTwoFactorTokenAsync(TwoFactorTokenType.LockAccount, user.Id, lockoutToken))
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Invalid Lockout Token", "The secure lockout token is invalid or has expired."));

			if (await UserManager.IsLockedOutAsync(user.Id))
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, "Account Locked", "Your account has already been locked.."));

			await UserManager.SetLockoutEndDateAsync(user.Id, DateTime.UtcNow.AddYears(1));
			await UserManager.UpdateSecurityStampAsync(user.Id);
			await EmailService.SendAsync(EmailType.UserLockout, user, Request.GetIPAddress());
			return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, "Account Lockdown!", "Your account has been locked at your request,"));
		}

		#endregion

		#region TwoFactor

		/// <summary>
		/// GET: Verifies the login two factor code.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> VerifyTwoFactor()
		{
			var userid = await GetTwoFactorLoginUserIdAsync();
			if (string.IsNullOrEmpty(userid))
				return Unauthorized();

			var user = await UserManager.FindByIdAsync(userid);
			if (user == null)
				return Unauthorized();

			var logonTwoFactor = user.TwoFactor.FirstOrDefault(x => x.Component == TwoFactorComponentType.Login);
			if (logonTwoFactor == null)
				return Unauthorized();

			return View("VerifyTwoFactor", new VerifyTwoFactorModel { TwoFactorType = logonTwoFactor.Type });
		}


		/// <summary>
		/// POST: Verifies the login two factor code.
		/// </summary>
		/// <param name="model">The model.</param>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> VerifyTwoFactor(VerifyTwoFactorModel model)
		{
			if (!ModelState.IsValid)
				return View("VerifyTwoFactor", model);

			string userId = await GetTwoFactorLoginUserIdAsync();
			if (userId == null)
				return Unauthorized();

			var user = await UserManager.FindByIdAsync(userId);
			if (user == null)
				return Unauthorized();

			var logonTwoFactor = user.TwoFactor.FirstOrDefault(x => x.Component == TwoFactorComponentType.Login);
			if (logonTwoFactor == null)
				return Unauthorized();

			if (await UserManager.VerifyUserTwoFactorCodeAsync(logonTwoFactor.Component, userId, model.Data))
			{
				await SignInAsync(user);
				return RedirectToLocal("Home");
			}

			await UserManager.AccessFailedAsync(user.Id);
			if (await UserManager.IsLockedOutAsync(user.Id))
			{
				await EmailService.SendAsync(EmailType.PasswordLockout, user, Request.GetIPAddress());
				ModelState.AddModelError("", "Your account is locked.");
				return View("Login");
			}

			ModelState.AddModelError("", "Invalid code");
			return View("VerifyTwoFactor", model);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Sets the two factor login authentication cookie.
		/// </summary>
		/// <param name="userId">The user identifier.</param>
		private void SetTwoFactorLoginAuthCookie(string userId)
		{
			var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
			AuthenticationManager.SignIn(identity);
		}

		/// <summary>
		/// Gets the two factor login user identifier asynchronous.
		/// </summary>
		/// <returns></returns>
		private async Task<string> GetTwoFactorLoginUserIdAsync()
		{
			var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorCookie);
			if (result != null && result.Identity != null && !string.IsNullOrEmpty(result.Identity.GetUserId()))
			{
				return result.Identity.GetUserId();
			}
			return null;
		}

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private async Task SignInAsync(ApplicationUser user)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
			var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
			AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true, }, identity);

			await UserManager.AddUserLogon(user, Request.GetIPAddress(), true);
			await EmailService.SendAsync(EmailType.Logon, user, Request.GetIPAddress(), GetLockoutLink(user));
		}


		//public enum ManageMessageId
		//{
		//	ChangePasswordSuccess,
		//	Error
		//}

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