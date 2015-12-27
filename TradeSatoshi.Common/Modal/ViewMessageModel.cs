
using System.Web.Mvc;
namespace TradeSatoshi.Common.Modal
{
	public class ViewMessageModel
	{
		public ViewMessageModel()
		{
		}
		public ViewMessageModel(ViewMessageType type, string title, string message, string returnUrl = "/")
		{
			Type = type;
			Title = title;
			Message = message;
			ReturnUrl = returnUrl;
		}

		public ViewMessageType Type { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string ReturnUrl { get; set; }

		public static ViewMessageModel Success(string title, string message, params object[] messageParams)
		{
			return new ViewMessageModel(ViewMessageType.Success, title, string.Format(message, messageParams));
		}

		public static ViewMessageModel Warning(string title, string message, params object[] messageParams)
		{
			return new ViewMessageModel(ViewMessageType.Warning, title, string.Format(message, messageParams));
		}

		public static ViewMessageModel Error(string title, string message, params object[] messageParams)
		{
			return new ViewMessageModel(ViewMessageType.Danger, title, string.Format(message, messageParams));
		}

		public static ViewMessageModel Info(string title, string message, params object[] messageParams)
		{
			return new ViewMessageModel(ViewMessageType.Info, title, string.Format(message, messageParams));
		}
	}

	public enum ViewMessageType
	{
		Info,
		Success,
		Warning,
		Danger
	}
}