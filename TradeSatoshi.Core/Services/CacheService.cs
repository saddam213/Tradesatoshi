using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services;

namespace TradeSatoshi.Core.Services
{
	public class CacheService : ICacheService
	{
		private static MemoryCache _cache = new MemoryCache("CacheService");

		public T GetOrSet<T>(string key, int seconds, Func<T> valueFactory)
		{
			return GetOrSet<T>(key, TimeSpan.FromSeconds(seconds), valueFactory);
		}

		public T GetOrSet<T>(string key, TimeSpan timespan, Func<T> valueFactory)
		{
		  var newValue = new Lazy<T>(valueFactory);
			var oldValue = _cache.AddOrGetExisting(key, newValue, DateTimeOffset.UtcNow.Add(timespan)) as Lazy<T>;
			try
			{
				return (oldValue ?? newValue).Value;
			}
			catch
			{
				_cache.Remove(key);
				throw;
			}
		}

		public async Task<T> GetOrSetASync<T>(string key, int seconds, Func<Task<T>> valueFactory)
		{
			return await GetOrSetASync<T>(key, TimeSpan.FromSeconds(seconds), valueFactory);
		}

		public async Task<T> GetOrSetASync<T>(string key, TimeSpan timespan, Func<Task<T>> valueFactory)
		{
			var newValue = new Lazy<Task<T>>(valueFactory);
			var oldValue = _cache.AddOrGetExisting(key, newValue, DateTimeOffset.UtcNow.Add(timespan)) as Lazy<Task<T>>;
			try
			{
				return await (oldValue ?? newValue).Value;
			}
			catch
			{
				_cache.Remove(key);
				throw;
			}
		}

	

		public void Invalidate(string key)
		{
			try
			{
				_cache.Remove(key);
			}
			catch (Exception)
			{
			}
		}
	}
}
