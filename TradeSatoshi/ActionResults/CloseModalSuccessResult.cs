using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradeSatoshi.Web.ActionResults
{
	public class CloseModalSuccessResult : ActionResult
	{
		public CloseModalSuccessResult(string message = null)
		{
			Message = message ?? string.Empty;
		}

		public string Message { get; set; }
		public override void ExecuteResult(ControllerContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = "text/html";
			response.Write(@"<script>(function () { $.modal.close({Success:true, Message:'" + Message + "'}); })();</script>");
		}
	}
}