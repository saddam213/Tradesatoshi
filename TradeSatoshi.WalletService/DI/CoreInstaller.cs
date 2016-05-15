using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.WalletService.Implementation;

namespace TradeSatoshi.WalletService
{
	public class CoreInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{

			container.Register(Classes.FromThisAssembly()
			.Pick()
			.WithService
			.DefaultInterfaces()
			.LifestyleTransient()
		);
			container.Register(Classes.FromAssemblyContaining<TradeSatoshi.Core.Core>()
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
			);

			container.Register(Classes.FromAssemblyContaining<TradeSatoshi.Data.Data>()
				.Pick()
				.WithService
				.DefaultInterfaces()
				.LifestyleTransient()
			);

		}
	}
}
