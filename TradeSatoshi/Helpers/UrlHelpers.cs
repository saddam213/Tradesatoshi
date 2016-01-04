using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradeSatoshi.Web.Helpers
{
	public static class UrlHelpers
	{
		private static string _modalStringFormat = "openModal('{0}');";

		public static string ActionModal(this UrlHelper helper, string actionName)
		{
			return string.Format(_modalStringFormat, helper.Action(actionName));
		}

		public static string ActionModal(this UrlHelper helper, string actionName, object routeValues)
		{
			return string.Format(_modalStringFormat, helper.Action(actionName, routeValues));
		}

		public static string ActionModal(this UrlHelper helper, string actionName, string controllerName)
		{
			return string.Format(_modalStringFormat, helper.Action(actionName, controllerName));
		}

		public static string ActionModal(this UrlHelper helper, string actionName, string controllerName, object routeValues)
		{
			return string.Format(_modalStringFormat, helper.Action(actionName, controllerName, routeValues));
		}
	}
}