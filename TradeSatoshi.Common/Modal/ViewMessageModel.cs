
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
	}

	public enum ViewMessageType
	{
		Info,
		Success,
		Warning,
		Danger
	}
}