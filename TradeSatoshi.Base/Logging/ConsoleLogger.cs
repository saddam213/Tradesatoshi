using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Base.Logging
{
	public class ConsoleLogger : Logger
	{
		public ConsoleLogger(LogLevel level)
			: base(level)
		{
		}

		protected override void LogQueuedMessage(string message)
		{
			Console.WriteLine(message);
		}
	}
}
