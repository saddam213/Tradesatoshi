using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradeSatoshi.Web.ActionResults
{
	public class CloseModalRedirectResult : ActionResult
	{
		public CloseModalRedirectResult(string redirectAction)
		{
			RedirectAction = redirectAction;
		}

		public string RedirectAction { get; set; }
		public override void ExecuteResult(ControllerContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = "text/html";
			response.Write(@"<script>(function () { $.modal.close(); location.href='" + RedirectAction + "'; })();</script>");
		}
	}
}