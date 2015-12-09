using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Validation
{
	public interface IWriterResult<T>
	{
		string Error { get; set; }
		string Message { get; set; }
		bool HasError { get; }
		bool HasMessage { get; }
		T Data { get; set; }
	}

	public class WriterResult<T> : IWriterResult<T>
	{
		public string Error { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }

		public bool HasError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}
	
		public bool HasMessage
		{
			get { return !string.IsNullOrEmpty(Message); }
		}


		public static WriterResult<T> SuccessResult(string message = null)
		{
			return new WriterResult<T> { Message = message };
		}

		public static WriterResult<T> ErrorResult(string error = null)
		{
			return new WriterResult<T> { Error = error };
		}

		public static WriterResult<T> SuccessResult(string message, params object[] param)
		{
			return new WriterResult<T> { Message = string.Format(message, param) };
		}

		public static WriterResult<T> ErrorResult(string error, params object[] param)
		{
			return new WriterResult<T> { Error = string.Format(error, param) };
		}

		public static WriterResult<T> SuccessResult(T data)
		{
			return new WriterResult<T> { Data = data };
		}

		public static WriterResult<T> SuccessResult(T data, string message)
		{
			return new WriterResult<T> { Data = data, Message = message };
		}
	}
}