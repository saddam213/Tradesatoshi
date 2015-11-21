using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Core;

namespace TradeSatoshi.DependencyInjection
{
	public class ControllersInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromThisAssembly()
				.BasedOn<IController>()
				.LifestyleTransient()
			);

			container.Register(Classes.FromAssemblyNamed("TradeSatoshi.Core")
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
			);
		}
	}

	//public class FooWindsorInstaller : IWindsorInstaller
	//{
	//	public void Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
	//	{

	//		container.Register(Component.For<ITestInjection>().ImplementedBy<TestInjection>());
	//	}
	//}
}