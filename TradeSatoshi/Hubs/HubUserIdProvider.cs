using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeSatoshi.Helpers;

namespace TradeSatoshi.Hubs
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