using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Base.Logging;

namespace TradeSatoshi.WalletService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			DependencyRegistrar.Register();
				var level = LoggingManager.LogLevelFromString(ConfigurationManager.AppSettings["LogLevel"]);
			var location = ConfigurationManager.AppSettings["LogLocation"];

#if DEBUG
			LoggingManager.AddLog(new ConsoleLogger(level));
			using (var processor = DependencyRegistrar.Resolve<WalletService>())
			{
				processor.StartService();
				Console.ReadLine();
				processor.StopService();
			}
#else
				LoggingManager.AddLog(new FileLogger(location, "WalletService", level));
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					DependencyRegistrar.Resolve<WalletService>()
				};
				ServiceBase.Run(ServicesToRun);
#endif
			DependencyRegistrar.Deregister();
		}
	}
}
