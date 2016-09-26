using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeSatoshi.Common.Validation
{
	public interface IApiResult<T>
	{
		bool Success { get; set; }
		string Message { get; set; }
		T Result { get; set; }
	}

	public class ApiResult<T> : IApiResult<T>
	{
		public ApiResult(bool success, T data)
		{
			Success = success;
			Result = data;
		}

		public ApiResult(bool success, string message)
		{
			Success = success;
			Message = message;
		}

		public ApiResult(bool success, string message, T data)
		{
			Success = success;
			Message = message;
			Result = data;
		}

		public ApiResult(Exception exception)
		{
			Success = false;
//			Message = "An unexpected error occured.";
//#if DEBUG
			Message = exception.ToString();
//#endif
		}

		public bool Success { get; set; }
		public string Message { get; set; }
		public T Result { get; set; }
	}
}