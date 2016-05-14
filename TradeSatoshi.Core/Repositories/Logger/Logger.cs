using System;
using System.Threading.Tasks;
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

		public async Task Info(string component, string message)
		{
			await LogMessage("Info", component, message);
		}

		public async Task Debug(string component, string message)
		{
			await LogMessage("Debug", component, message);
		}

		public async Task Warn(string component, string message)
		{
			await LogMessage("Warn", component, message );
		}

		public async Task Error(string component, string message)
		{
			await LogMessage("Error", component, message);
		}

		public async Task Exception(string component, Exception ex)
		{
			await LogMessage("Error", component, ex.ToString());
		}

		public async Task Exception(string component, Exception ex, string message)
		{
			await LogMessage("Error", component, string.Concat(message, Environment.NewLine, ex.ToString()));
		}

		private async Task LogMessage(string type, string component, string message)
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
				await dataContext.SaveChangesAsync();
			}
		}
	}
}