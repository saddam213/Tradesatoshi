using System;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Logging;

namespace TradeSatoshi.Core.Logger
{
	public class DatabaseLogger : ILogger
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public DatabaseLogger()
		{
		}

		public DatabaseLogger(IDataContextFactory dataContextFactory)
		{
			DataContextFactory = dataContextFactory;
		}

		public void Info(string component, string message, params object[] formatParams)
		{
			LogMessage("Info", component, string.Format(message, formatParams));
		}

		public void Debug(string component, string message, params object[] formatParams)
		{
			LogMessage("Debug", component, string.Format(message, formatParams));
		}

		public void Warn(string component, string message, params object[] formatParams)
		{
			LogMessage("Warn", component, string.Format(message, formatParams));
		}

		public void Error(string component, string message, params object[] formatParams)
		{
			LogMessage("Error", component, string.Format(message, formatParams));
		}

		public void Exception(string component, Exception ex)
		{
			LogMessage("Error", component, ex.ToString());
		}

		public void Exception(string component, Exception ex, string message, params object[] formatParams)
		{
			LogMessage("Error", component, string.Concat(string.Format(message, formatParams), Environment.NewLine, ex.ToString()));
		}

		private void LogMessage(string type, string component, string message)
		{
			using (var dataContext = DataContextFactory.CreateContext())
			{
				dataContext.Log.Add(new Entity.Log
				{
					Component = component,
					Message = message,
					Type = type,
					Timestamp = DateTime.UtcNow
				});
				dataContext.SaveChanges();
			}
		}
	}
}