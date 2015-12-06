using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Base.Extensions
{
	public static class LinqExtensions
	{
		public static bool IsNullOrEmpty<TResult>(this IEnumerable<TResult> items)
		{
			if (items == null)
			{
				return true;
			}
			return !items.Any();
		}
	}
}
