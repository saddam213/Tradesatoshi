using System.Web.Mvc;

namespace TradeSatoshi.Web.ActionResults
{
	public class CloseModalResult : ActionResult
	{
		public CloseModalResult() { }
		public CloseModalResult(string redirectAction)
		{
			RedirectAction = redirectAction;
		}
		public string RedirectAction { get; set; }
		public override void ExecuteResult(ControllerContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = "text/html";
			if (!string.IsNullOrEmpty(RedirectAction))
			{
				response.Write(@"<script>(function () { $.modal.close(); location.href='" + RedirectAction + "'; })();</script>");
			}
			else
			{
				response.Write(@"<script>(function () { $.modal.close(); })();</script>");
			}
		}
	}
}