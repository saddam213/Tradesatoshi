using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace TradeSatoshi.Web.ActionResults
{
	public class CloseModalResult : ActionResult
	{
		public CloseModalResult() { }
		public override void ExecuteResult(ControllerContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = "text/html";
			response.Write(@"<script>(function () { $.modal.close(); })();</script>");
		}
	}
}