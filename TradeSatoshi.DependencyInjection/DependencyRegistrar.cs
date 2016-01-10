using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using TradeSatoshi.DependencyInjection.MVC;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;

namespace TradeSatoshi.DependencyInjection
{
	public static class DependencyRegistrar
	{
		private static IWindsorContainer _container;

		static DependencyRegistrar()
		{
			if (_container == null)
			{
				_container = BootstrapContainer();
			}
		}

		private static IWindsorContainer BootstrapContainer()
		{
			IWindsorContainer container = new WindsorContainer();
			container.Install(FromAssembly.This());
			return container;
		}

		public static void Register()
		{
			var cryptopiaDependencyResolver = new MVCDependencyResolver(_container.Kernel);
			DependencyResolver.SetResolver(cryptopiaDependencyResolver);
			GlobalHost.DependencyResolver = new SignalrDependencyResolver(_container.Kernel);
		}

		public static void Deregister()
		{
			_container.Dispose();
		}

		public static void RegisterTransientComponent<T>(Func<T> factoryCreate) where T : class
		{
			_container
				.Register(Component.For<T>()
				.UsingFactoryMethod(factoryCreate)
				.LifestyleTransient());
		}
	}
}
