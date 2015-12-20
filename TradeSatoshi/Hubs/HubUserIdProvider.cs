using Microsoft.AspNet.SignalR;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Hubs
{
	public class HubUserIdProvider : IUserIdProvider
	{
		public string GetUserId(IRequest request)
		{
			if (request.User != null)
			{
				return request.User.Id();
			}
			return string.Empty;
		}
	}
}