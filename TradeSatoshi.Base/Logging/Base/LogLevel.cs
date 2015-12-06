using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Base.Logging
{
	public enum LogLevel : int
	{
		Verbose = 0,
		/// <summary>
		/// Debug message
		/// </summary>
		Debug = 1,
		/// <summary>
		/// Info message
		/// </summary>
		Info = 2,
		/// <summary>
		/// Warn message
		/// </summary>
		Warn = 3,
		/// <summary>
		/// Error message
		/// </summary>
		Error = 4,
		/// <summary>
		/// No Logging
		/// </summary>
		None = 5,
	}
}
