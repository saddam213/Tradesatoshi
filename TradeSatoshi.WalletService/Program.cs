using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.WalletService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
#if DEBUG
			using (var processor = new WalletService())
			{
				processor.StartService();
				Console.WriteLine("Press Enter to terminate ...");
				Console.ReadLine();
				processor.StopService();
			}
#else

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new WalletService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
		}
	}
}
