using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using TradeSatoshi.App_Start;
using TradeSatoshi.Common;
using TradeSatoshi.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Threading.Tasks;
using TradeSatoshi.Data;
using TradeSatoshi.ActionResults;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Data.Entities;

namespace TradeSatoshi.Controllers
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

		protected JsonResult JsonSuccess(string message = null)
		{
			return Json(new { Success = true, Message = message });
		}

		protected JsonResult JsonError(string message = null)
		{
			return Json(new { Success = false, Message = message });
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

		protected CloseModalResult CloseModal()
		{
			return new CloseModalResult();
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