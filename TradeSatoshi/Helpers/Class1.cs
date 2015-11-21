using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;

namespace TradeSatoshi
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

		//private static string GetResourceString(HttpContextBase httpContext, string expression, string virtualPath, object[] args)
		//{
		//	ExpressionBuilderContext context = new ExpressionBuilderContext(virtualPath);
		//	ResourceExpressionBuilder builder = new ResourceExpressionBuilder();
		//	ResourceExpressionFields fields = (ResourceExpressionFields)builder.ParseExpression(expression, typeof(string), context);

		//	if (!string.IsNullOrEmpty(fields.ClassKey))
		//		return string.Format((string)httpContext.GetGlobalResourceObject(fields.ClassKey, fields.ResourceKey, CultureInfo.CurrentUICulture), args);

		//	return string.Format((string)httpContext.GetLocalResourceObject(virtualPath, fields.ResourceKey, CultureInfo.CurrentUICulture), args);
		//}

		//private static string GetVirtualPath(HtmlHelper htmlhelper)
		//{
		//	WebFormView view = htmlhelper.ViewContext.View as WebFormView;

		//	if (view != null)
		//		return view.ViewPath;

		//	return null;
		//}
	}
}