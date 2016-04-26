using System.Web.Http;
using hbehr.recaptcha;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.DependencyInjection;

namespace TradeSatoshi.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			DependencyRegistrar.Register();
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (RequiredIfAttribute), typeof (RequiredAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (RequiredToBeTrueAttribute), typeof (RequiredAttributeAdapter));
			ModelBinders.Binders.Add(typeof (TradeSatoshi.Common.DataTables.DataTablesModel), new TradeSatoshi.Web.ModelBinder.DataTablesModelBinder());
			string publicKey = "6LdfdBETAAAAAILHIQ4yjZST5zbTPEhcIBSPA8Ld";
			string secretKey = "6LdfdBETAAAAAI1d_wuXstow54r4eR4AKVWLRZle";
			ReCaptcha.Configure(publicKey, secretKey);
		}

		protected void Application_End()
		{
			DependencyRegistrar.Deregister();
		}
	}
}