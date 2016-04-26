using System.Web.Optimization;

namespace TradeSatoshi.Web
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/jquery.unobtrusive-ajax.js",
				"~/Scripts/jquery.signalR-2.2.0.js",
				"~/Scripts/bootstrap.js",
				"~/Scripts/respond.js",
				"~/Scripts/jquery.dataTables.js",
				"~/Scripts/dataTables.bootstrap.js",
				"~/Scripts/Project/notifications.js",
				"~/Scripts/Project/simplemodal.js",
				"~/Scripts/mustache.js",
				"~/Scripts/history.js",
				"~/Scripts/moment.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/modernizr-*"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/bootstrap.css",
				"~/Content/dataTables.bootstrap.css",
				"~/Content/whhg.css",
				"~/Content/notification.css",
				"~/Content/Site.css",
				"~/Content/Site-theme.css"));
		}
	}
}