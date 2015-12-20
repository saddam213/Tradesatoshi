using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace TradeSatoshi.Web
{
	public static class ResourceHelper
	{
		public static string Resource(this HtmlHelper htmlhelper, string expression, params object[] args)
		{
			return string.Format(expression, args);
		}

		public static string Resource(this Controller controller, string expression, params object[] args)
		{
			return string.Format(expression, args);
		}

		public static MvcHtmlString ResourceLabel(this HtmlHelper helper, string label, object htmlAttributes = null)
		{
			return helper.Label(label, htmlAttributes);
		}

		public static MvcHtmlString ResourceLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
		{
			return helper.LabelFor(expression, htmlAttributes);
		}

		public static MvcHtmlString ResourceActionLink(this HtmlHelper htmlHelper, string linkText, string actionName)
		{
			return htmlHelper.ActionLink(linkText, actionName);
		}

		public static MvcHtmlString ResourceActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues)
		{
			return htmlHelper.ActionLink(linkText, actionName, routeValues);
		}

		public static MvcHtmlString ResourceActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
		{
			return htmlHelper.ActionLink(linkText, actionName, controllerName);
		}

		public static MvcHtmlString ResourceActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues)
		{
			return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues);
		}

		public static MvcHtmlString ResourceActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
		{
			return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
		}

	}
}