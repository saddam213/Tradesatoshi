using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TradeSatoshi.DependencyInjection.MVC
{
	public class MVCDependencyResolver : IDependencyResolver
	{
		private readonly Castle.MicroKernel.IKernel _kernal;
		public MVCDependencyResolver(Castle.MicroKernel.IKernel kernal)
		{
			_kernal = kernal;
		}

		public object GetService(Type serviceType)
		{
			return _kernal.HasComponent(serviceType) ? _kernal.Resolve(serviceType) : null;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _kernal.ResolveAll(serviceType).Cast<object>();
		}
	}
}
