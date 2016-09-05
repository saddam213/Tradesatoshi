using System.Web.Http;
using hbehr.recaptcha;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json.Serialization;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.DependencyInjection;
using System.Configuration;

namespace TradeSatoshi.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		private readonly string _ReCaptchaPublicKey = ConfigurationManager.AppSettings["ReCaptchaPublicKey"];
		private readonly string _ReCaptchaPrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];
		protected void Application_Start()
		{
			DependencyRegistrar.Register();
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (RequiredIfAttribute), typeof (RequiredAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (RequiredToBeTrueAttribute), typeof (RequiredAttributeAdapter));
			ModelBinders.Binders.Add(typeof (TradeSatoshi.Common.DataTables.DataTablesModel), new TradeSatoshi.Web.ModelBinder.DataTablesModelBinder());
			ReCaptcha.Configure(_ReCaptchaPublicKey, _ReCaptchaPrivateKey);
		}

		protected void Application_End()
		{
			DependencyRegistrar.Deregister();
		}
	}
}