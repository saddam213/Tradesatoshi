using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Validation
{
	public interface IWriterResult<T>
	{
		List<string> Errors { get; set; }
		string Message { get; set; }
		bool HasErrors { get; }
		bool HasMessage { get; }
		string FlattenErrors { get; }
		string FirstError { get; }
		T Data { get; set; }
	}

	public class WriterResult<T> : IWriterResult<T>
	{
		public WriterResult()
		{
			Errors = new List<string>();
		}

		public List<string> Errors { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }

		public bool HasErrors
		{
			get { return Errors.Any(); }
		}

		public bool HasMessage
		{
			get { return !string.IsNullOrEmpty(Message); }
		}

		public string FlattenErrors
		{
			get { return string.Join(Environment.NewLine, Errors); }
		}

		public string FirstError
		{
			get { return Errors.FirstOrDefault(); }
		}


		public static WriterResult<T> SuccessResult(string message = null)
		{
			return new WriterResult<T> { Message = message };
		}

		public static WriterResult<T> ErrorResult(string error = null)
		{
			return new WriterResult<T> { Errors = new List<string> { error } };
		}

		public static WriterResult<T> SuccessResult(string message, params object[] param)
		{
			return new WriterResult<T> { Message = string.Format(message, param) };
		}

		public static WriterResult<T> ErrorResult(string error, params object[] param)
		{
			return new WriterResult<T> { Errors = new List<string> { string.Format(error, param) } };
		}

		public static WriterResult<T> ErrorResult(List<string> errors)
		{
			return new WriterResult<T> { Errors = errors };
		}

		public static WriterResult<T> SuccessResult(T data)
		{
			return new WriterResult<T> { Data = data };
		}

		public static WriterResult<T> SuccessResult(T data, string message)
		{
			return new WriterResult<T> { Data = data, Message = message };
		}

	

		public static WriterResult<T> ContextResult(List<string> contextResults)
		{
			if (contextResults.Any())
			{
				return WriterResult<T>.ErrorResult(contextResults);
			}
			return WriterResult<T>.SuccessResult();
		}

		public static WriterResult<T> ContextResult(List<string> contextResults, string successMessage, params object[] formatParams)
		{
			if (contextResults.Any())
			{
				return WriterResult<T>.ErrorResult(contextResults);
			}
			return WriterResult<T>.SuccessResult(successMessage, formatParams);
		}

		public static WriterResult<T> ContextResult(T data, List<string> contextResults)
		{
			if (contextResults.Any())
			{
				return WriterResult<T>.ErrorResult(contextResults);
			}
			return WriterResult<T>.SuccessResult(data);
		}

		public static WriterResult<T> ContextResult(T data, List<string> contextResults, string successMessage)
		{
			if (contextResults.Any())
			{
				return WriterResult<T>.ErrorResult(contextResults);
			}
			return WriterResult<T>.SuccessResult(data, successMessage);
		}
	}
}