using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services
{
	public interface ICacheService
	{
		T GetOrSet<T>(string key, int seconds, Func<T> valueFactory);
		T GetOrSet<T>(string key, TimeSpan timespan, Func<T> valueFactory);
		Task<T> GetOrSetASync<T>(string key, int seconds, Func<Task<T>> valueFactory);
		Task<T> GetOrSetASync<T>(string key, TimeSpan timespan, Func<Task<T>> valueFactory);

		void Invalidate(string key);
	}
}
