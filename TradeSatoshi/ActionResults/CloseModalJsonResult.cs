using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace TradeSatoshi.Web.ActionResults
{
	public class CloseModalJsonResult : ActionResult
	{
		public CloseModalJsonResult(object data)
		{
			var serializer = new JavaScriptSerializer();
			JsonData = serializer.Serialize(data);
		}

		public string JsonData { get; set; }
		public override void ExecuteResult(ControllerContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = "text/html";
			if (!string.IsNullOrEmpty(JsonData))
			{
				response.Write(@"<script>(function () { $.modal.close(" + JsonData + "); })();</script>");
			}
			else
			{
				response.Write(@"<script>(function () { $.modal.close(); })();</script>");
			}
		}
	}
}