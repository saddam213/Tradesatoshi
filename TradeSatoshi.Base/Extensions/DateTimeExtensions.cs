using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Base.Extensions
{
	public static class DateTimeExtensions
	{
		public static DateTime ToDateTime(this uint time)
		{
			return new DateTime(1970, 1, 1).AddSeconds(time);
		}

		public static long ToJavaTime(this DateTime date)
		{
			var timeSpan = (date - new DateTime(1970, 1, 1, 0, 0, 0));
			return (long)timeSpan.TotalMilliseconds;
		}

		public static long ToUnixTime(this DateTime date)
		{
			var timeSpan = (date - new DateTime(1970, 1, 1, 0, 0, 0));
			return (long)timeSpan.TotalSeconds;
		}
	}
}
