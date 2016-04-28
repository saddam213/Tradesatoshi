using System.Net.Http.Headers;
using System.Web.Http;

namespace TradeSatoshi.Web
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

			config.Routes.MapHttpRoute(name: "GetCurrencies", routeTemplate: "api/public/getcurrencies", defaults: new {controller = "Public", action = "GetCurrencies"});
			config.Routes.MapHttpRoute(name: "GetTicker", routeTemplate: "api/public/getticker", defaults: new {controller = "Public", action = "GetTicker"});
			config.Routes.MapHttpRoute(name: "GetMarketHistory", routeTemplate: "api/public/getmarkethistory", defaults: new {controller = "Public", action = "GetMarketHistory"});
			config.Routes.MapHttpRoute(name: "GetMarketSummary", routeTemplate: "api/public/getmarketsummary", defaults: new {controller = "Public", action = "GetMarketSummary"});
			config.Routes.MapHttpRoute(name: "GetMarketSummaries", routeTemplate: "api/public/getmarketsummaries", defaults: new {controller = "Public", action = "GetMarketSummaries"});
			config.Routes.MapHttpRoute(name: "GetOrderBook", routeTemplate: "api/public/getorderbook", defaults: new {controller = "Public", action = "GetOrderBook"});

			config.Routes.MapHttpRoute(name: "GetOrder", routeTemplate: "api/private/getorder", defaults: new { controller = "Private", action = "GetOrder" });
			config.Routes.MapHttpRoute(name: "GetOrders", routeTemplate: "api/private/getorders", defaults: new { controller = "Private", action = "GetOrders" });
			config.Routes.MapHttpRoute(name: "GetTradeHistory", routeTemplate: "api/private/gettradehistory", defaults: new { controller = "Private", action = "GetTradeHistory" });
			config.Routes.MapHttpRoute(name: "GetBalance", routeTemplate: "api/private/getbalance", defaults: new { controller = "Private", action = "GetBalance" });
			config.Routes.MapHttpRoute(name: "GetBalances", routeTemplate: "api/private/getbalances", defaults: new { controller = "Private", action = "GetBalances" });
			config.Routes.MapHttpRoute(name: "GetWithdrawals", routeTemplate: "api/private/getwithdrawals", defaults: new { controller = "Private", action = "GetWithdrawals" });
			config.Routes.MapHttpRoute(name: "GetDeposits", routeTemplate: "api/private/getdeposits", defaults: new { controller = "Private", action = "GetDeposits" });

			config.Routes.MapHttpRoute(name: "SubmitWithdraw", routeTemplate: "api/private/submitwithdraw", defaults: new { controller = "Private", action = "SubmitWithdraw" });
			config.Routes.MapHttpRoute(name: "GenerateAddress", routeTemplate: "api/private/generateaddress", defaults: new { controller = "Private", action = "GenerateAddress" });
			config.Routes.MapHttpRoute(name: "CancelOrders", routeTemplate: "api/private/cancelorders", defaults: new { controller = "Private", action = "CancelOrders" });
			config.Routes.MapHttpRoute(name: "CancelOrder", routeTemplate: "api/private/cancelorder", defaults: new { controller = "Private", action = "CancelOrder" });
			config.Routes.MapHttpRoute(name: "SubmitOrder", routeTemplate: "api/private/submitorder", defaults: new { controller = "Private", action = "SubmitOrder" });
		}
	}
}