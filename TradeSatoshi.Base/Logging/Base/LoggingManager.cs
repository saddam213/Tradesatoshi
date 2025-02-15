﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Base.Logging
{
	public static class LoggingManager
	{
		private static readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();
		private static readonly Dictionary<string, Dictionary<Type, Log>> _logs = new Dictionary<string, Dictionary<Type, Log>>();

		public static void AddLog(Logger logger, string logname = "Default")
		{
			if (_loggers.ContainsKey(logname))
			{
				_loggers[logname].Dispose();
				_loggers.Remove(logname);
			}

			_loggers.Add(logname, logger);
			_logs.Add(logname, new Dictionary<Type, Log>());
			logger.WriteHeader();
		}

		public static Log GetLog(Type owner, string logName = "Default")
		{
			if (_loggers.Count == 0)
			{
				AddLog(new ConsoleLogger(LogLevel.Verbose));
			}

			if (!_loggers.ContainsKey(logName) || !_logs.ContainsKey(logName)) return _logs[logName][owner];
			if (!_logs[logName].ContainsKey(owner))
			{
				_logs[logName].Add(owner, new Log(owner, _loggers[logName].QueueLogMessage));
			}
			return _logs[logName][owner];
		}

		/// <summary>
		/// Destroys the logging manager
		/// </summary>
		public static void Destroy()
		{
			if (!_loggers.Any()) return;
			foreach (var logger in _loggers.Values)
			{
				logger.Dispose();
			}
			_loggers.Clear();
			_logs.Clear();
		}

		public static LogLevel LogLevelFromString(string level)
		{
			switch (level)
			{
				case "Verbose":
					return LogLevel.Verbose;
				case "Debug":
					return LogLevel.Debug;
				case "Info":
					return LogLevel.Info;
				case "Warn":
					return LogLevel.Warn;
				case "Error":
					return LogLevel.Error;
				case "None":
					return LogLevel.None;
			}
			return LogLevel.Verbose;
		}
	}
}
