using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using TradeSatoshi.App_Start;
using TradeSatoshi.Common;
using TradeSatoshi.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Web;

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

		protected PartialViewResult PartialViewMessage(ViewMessageModel model)
		{
			return PartialView("ViewMessage", model);
		}

		protected ViewResult Unauthorized()
		{
			return View("Unauthorized");
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