using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Validation
{
	public interface IWriterResult
	{
		string Error { get; set; }
		string Message { get; set; }
		bool HasError { get; }
		bool HasMessage { get; }
	}

	public class WriterResult : IWriterResult
	{
		public string Error { get; set; }
		public string Message { get; set; }

		public bool HasError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}
	
		public bool HasMessage
		{
			get { return !string.IsNullOrEmpty(Message); }
		}


		public static WriterResult SuccessResult(string message = null)
		{
			return new WriterResult { Message = message };
		}

		public static WriterResult ErrorResult(string error = null)
		{
			return new WriterResult { Error = error };
		}

		public static WriterResult SuccessResult(string message, params object[] param)
		{
			return new WriterResult { Message = string.Format(message, param) };
		}

		public static WriterResult ErrorResult(string error, params object[] param)
		{
			return new WriterResult { Error = string.Format(error, param) };
		}
	}
}