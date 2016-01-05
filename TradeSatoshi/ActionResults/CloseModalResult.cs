using System.Web.Mvc;
using System.Web.Script.Serialization;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Web.ActionResults
{
	public class CloseModalResult : ActionResult
	{
		public CloseModalResult() { }
		public CloseModalResult(object data)
		{
			var serializer = new JavaScriptSerializer();
			JsonData = serializer.Serialize(data);
		}
		public CloseModalResult(bool success = true, string message = null, string title = null, AlertType alertType = AlertType.Success)
		{
			var serializer = new JavaScriptSerializer();
			JsonData = serializer.Serialize(new
			{
				Success = success,
				Message = message,
				Title = title,
				AlertType = alertType.ToString()
			});
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