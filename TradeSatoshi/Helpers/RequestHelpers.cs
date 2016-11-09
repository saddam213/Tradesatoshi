using System;
using System.Linq;
using System.Net;
using System.Web;

namespace TradeSatoshi.Web.Helpers
{
	public static class RequestHelpers
	{
		public static string GetIPAddress(this HttpRequestBase request)
		{
			string defaultHostAddress = request.UserHostAddress;
			string cloudfareHostAddress = request.ServerVariables["HTTP_CF_CONNECTING_IP"];
			string proxyHostAddress = request.ServerVariables["X_FORWARDED_FOR"];
		
			if (!string.IsNullOrEmpty(cloudfareHostAddress))
				return cloudfareHostAddress;

			if (!string.IsNullOrEmpty(proxyHostAddress))
			{
				if (proxyHostAddress.IndexOf(",") > 0)
				{
					var ips = proxyHostAddress.Split(',');
					foreach (string ip in ips)
					{
						if (!IsLocalHost(ip))
							return ip;
					}
				}
				return proxyHostAddress;
			}

			if (!string.IsNullOrEmpty(defaultHostAddress))
				return defaultHostAddress;

			return "0.0.0.0";
		}

		private static bool IsLocalHost(string input)
		{
			IPAddress[] host;
			//get host addresses
			try { host = Dns.GetHostAddresses(input); }
			catch (Exception) { return false; }
			//get local adresses
			IPAddress[] local = Dns.GetHostAddresses(Dns.GetHostName());
			//check if local
			return host.Any(hostAddress => IPAddress.IsLoopback(hostAddress) || local.Contains(hostAddress));
		}

	}
}