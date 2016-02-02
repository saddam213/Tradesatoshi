using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Data;
using TradeSatoshi.Entity;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.ActionResults;
using TradeSatoshi.Web.App_Start;

namespace TradeSatoshi.Web.Controllers
{
	public class BaseController : Controller
	{
		private ApplicationUserManager _userManager;
		public BaseController()
		{
		}

		public BaseController(ApplicationUserManager userManager)
		{
			//UserManager = userManager;
		}

		public ApplicationUserManager UserManager
		{
			get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
			private set { _userManager = value; }
		}

		protected JsonResult JsonSuccess(string message = null, string title = null, AlertType alertType = AlertType.Success)
		{
			return Json(new { Success = true, Message = message, Title = title, AlertType = alertType.ToString() });
		}

		protected JsonResult JsonError(string message = null, string title = null, AlertType alertType = AlertType.Danger)
		{
			return Json(new { Success = false, Message = message, Title = title, AlertType = alertType.ToString() });
		}

		protected ViewResult ViewMessage(ViewMessageModel model)
		{
			return View("ViewMessage", model);
		}

		protected ViewResult ViewMessageModal(ViewMessageModel model)
		{
			return View("ViewMessageModal", model);
		}

		protected PartialViewResult ViewMessagePartial(ViewMessageModel model)
		{
			return PartialView("ViewMessagePartial", model);
		}

		protected ViewResult Unauthorized()
		{
			return View("Unauthorized");
		}

		protected ViewResult UnauthorizedModal()
		{
			return View("UnauthorizedModal");
		}

		protected CloseModalResult CloseModal()
		{
			return new CloseModalResult();
		}

		protected CloseModalResult CloseModal(object data)
		{
			return new CloseModalResult(data);
		}

		protected CloseModalResult CloseModalSuccess(string message = null, string title = null, AlertType alertType = AlertType.Success)
		{
			return new CloseModalResult(true, message, title, alertType);
		}

		protected CloseModalResult CloseModalError(string message = null, string title = null, AlertType alertType = AlertType.Danger)
		{
			return new CloseModalResult(false, message, title, alertType);
		}

		protected CloseModalRedirectResult CloseModalRedirect(string redirectAction)
		{
			return new CloseModalRedirectResult(redirectAction);
		}

		protected DataTablesResult DataTable(DataTablesResponse dataTablesResponse)
		{
			return new DataTablesResult(dataTablesResponse);
		}

		protected async Task<string> GetLockoutLink(ApplicationUser user)
		{
			return Url.Action("LockAccount", "Account", new
			{
				username = user.UserName,
				lockoutToken = await UserManager.GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.LockAccount, user.Id)
			}, protocol: Request.Url.Scheme);
		}

		protected internal ActionResult RedirectToActionWithHash(string actionName, string controllerName, string hash)
		{
			return new RedirectResult(Url.Action(actionName, controllerName) + hash);
		}

		protected internal void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}
		//protected override void Dispose(bool disposing)
		//{
		//	if (disposing && UserManager != null)
		//	{
		//		UserManager.Dispose();
		//		UserManager = null;
		//	}
		//	base.Dispose(disposing);
		//}
	}
}