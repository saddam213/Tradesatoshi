using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeSatoshi.Web.HttpModules
{
	public class SignalRFormsAuthenticationCleanerModule : IHttpModule
	{
		public void Init(HttpApplication application)
		{
			application.PreSendRequestHeaders += OnPreSendRequestHeaders;
		}

		private bool ShouldCleanResponse(string path)
		{
			path = path.ToLower();
			var urlsToClean = new string[] { "/signalr/" };


			// Check for a Url match
			foreach (var url in urlsToClean)
			{
				var result = path.IndexOf(url, StringComparison.OrdinalIgnoreCase) > -1;
				if (result)
					return true;
			}

			return false;
		}

		protected void OnPreSendRequestHeaders(object sender, EventArgs e)
		{
			var httpContext = ((HttpApplication)sender).Context;
			if (ShouldCleanResponse(httpContext.Request.Path))
			{
				// Remove Auth Cookie from response
				var formData = httpContext.Request.Form["data"];
				if (!string.IsNullOrEmpty(formData) && formData.IndexOf("GetOnlineCount", StringComparison.OrdinalIgnoreCase) > -1)
				{
					httpContext.Response.Cookies.Remove(DefaultAuthenticationTypes.ApplicationCookie);
					return;
				}
			}
		}

		public void Dispose()
		{
		}
	}
}