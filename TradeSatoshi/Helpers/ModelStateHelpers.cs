using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Web.Helpers
{
	public static class ModelStateHelpers
	{
		public static bool IsWriterResultValid<T>(this ModelStateDictionary modelState, IWriterResult<T> writerResult) where T : IConvertible
		{
			if (writerResult.HasErrors)
			{
				foreach (var item in writerResult.Errors)
				{
					modelState.AddModelError("", item);
				}
			}
			return modelState.IsValid;
		}
	}
}