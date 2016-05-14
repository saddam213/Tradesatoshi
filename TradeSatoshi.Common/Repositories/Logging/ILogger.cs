using System;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Logging
{
	public interface ILogger
	{
		Task Info(string component, string message);
		Task Debug(string component, string message);
		Task Warn(string component, string message);
		Task Error(string component, string message);
		Task Exception(string component, Exception ex);
		Task Exception(string component, Exception ex, string message);
	}
}