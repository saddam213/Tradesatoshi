using System.Web.Mvc;
using System.Web.Routing;

namespace TradeSatoshi.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute(
				name: "ChatWindow",
				url: "Chat/Chat",
				defaults: new { controller = "Chat", action = "Chat" }
			);

			routes.MapRoute(name: "User", url: "User", defaults: new { controller = "User", action = "Index", section = "Account" });
			routes.MapRoute(name: "Account", url: "Account", defaults: new { controller = "User", action = "Index", section = "Account" });
			routes.MapRoute(name: "Security", url: "Security", defaults: new { controller = "User", action = "Index", section = "Security" });
			routes.MapRoute(name: "Settings", url: "Settings", defaults: new { controller = "User", action = "Index", section = "Settings" });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
