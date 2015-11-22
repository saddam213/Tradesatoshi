using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using hbehr.recaptcha;
using System.Data.Entity.Validation;

namespace TradeSatoshi.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		public AccountController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
		{
		}

		public AccountController(UserManager<ApplicationUser> userManager)
		{
			UserManager = userManager;
		}

		public UserManager<ApplicationUser> UserManager { get; private set; }

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindAsync(model.UserName, model.Password);
				if (user != null)
				{
					await SignInAsync(user, model.RememberMe);
					return RedirectToLocal(returnUrl);
				}
				else
				{
					ModelState.AddModelError("", this.Resource("Invalid username or password."));
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
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

				try
				{
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
						await SignInAsync(user, isPersistent: false);
						return RedirectToAction("Index", "Home");
					}
					else
					{
						AddErrors(result);
					}
				}
				catch (DbEntityValidationException ex)
				{

					throw;
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/Manage
		public ActionResult Manage(ManageMessageId? message)
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? this.Resource("Your password has been changed.")
				: message == ManageMessageId.RemoveLoginSuccess ? this.Resource("The external login was removed.")
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
					State = user.Profile.State
				}
			});
		}

		//
		// POST: /Account/Manage
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
		public async Task<ActionResult> Profile(UserProfileModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("Manage", new ManageUserViewModel { Profile = model });
			}

			var user = UserManager.FindById(User.Identity.GetUserId());
			user.Profile.Address = model.Address;
			user.Profile.BirthDate = model.BirthDate;
			user.Profile.City = model.City;
			user.Profile.Country = model.Country;
			user.Profile.FirstName = model.FirstName;
			user.Profile.LastName = model.LastName;
			user.Profile.PostCode = model.PostCode;
			user.Profile.State = model.State;
			await UserManager.UpdateAsync(user);

			return View("Manage", new ManageUserViewModel { Profile = model });
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}




		protected override void Dispose(bool disposing)
		{
			if (disposing && UserManager != null)
			{
				UserManager.Dispose();
				UserManager = null;
			}
			base.Dispose(disposing);
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

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
			RemoveLoginSuccess,
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