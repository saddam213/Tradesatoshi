using System;

namespace TradeSatoshi.Common.Logging
{
	public interface ILogger
	{
		void Info(string component, string message, params object[] formatParams);
		void Debug(string component, string message, params object[] formatParams);
		void Warn(string component, string message, params object[] formatParams);
		void Error(string component, string message, params object[] formatParams);
		void Exception(string component, Exception ex);
		void Exception(string component, Exception ex, string message, params object[] formatParams);
	}
}