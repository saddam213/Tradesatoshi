using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using hbehr.recaptcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TradeSatoshi.DependencyInjection;
using TradeSatoshi.Validation;

namespace TradeSatoshi
{
	public class MvcApplication : System.Web.HttpApplication
	{
		private static IWindsorContainer container;

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredIfAttribute), typeof(RequiredAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredToBeTrueAttribute), typeof(RequiredAttributeAdapter));
			ModelBinders.Binders.Add(typeof(TradeSatoshi.Common.DataTables.DataTablesModel), new TradeSatoshi.Models.DataTablesModelBinder());
			BootstrapContainer();
			string publicKey = "6LdfdBETAAAAAILHIQ4yjZST5zbTPEhcIBSPA8Ld";
			string secretKey = "6LdfdBETAAAAAI1d_wuXstow54r4eR4AKVWLRZle";
			ReCaptcha.Configure(publicKey, secretKey);
		}

		protected void Application_End()
		{
			container.Dispose();
		}

		private static void BootstrapContainer()
		{
			container = new WindsorContainer()
				.Install(FromAssembly.This());
			var controllerFactory = new ControllerFactory(container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);
		}
	}
}
