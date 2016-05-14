using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Data.DataContext;

namespace TradeSatoshi.Web.Api.Authentication
{
	public class ApiKeyStore
	{
		private static MemoryCache _apiKeyCache = new MemoryCache("ApiKeyStore");

		public static UserApiAuthKey GetApiAuthKey(string apiKey)
		{
			try
			{
				var userApiAuthKey = _apiKeyCache.Get(apiKey) as UserApiAuthKey;
				if (userApiAuthKey == null)
				{
					using (var context = ApplicationDbContext.Create())
					{
						userApiAuthKey = context.Users
						.Where(x => x.IsApiEnabled && x.ApiKey == apiKey)
						.Select(user => new UserApiAuthKey
						{
							Key = user.ApiKey,
							Secret = user.ApiSecret,
							UserId = user.Id,
							IsEnabled = user.IsApiEnabled
						}).FirstOrDefault();

						if (userApiAuthKey != null)
						{
							_apiKeyCache.AddOrGetExisting(apiKey, userApiAuthKey, DateTimeOffset.UtcNow.AddDays(1));
						}
					}
				}
				return userApiAuthKey;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static void InvalidateApiKey(string oldApiKey)
		{
			try
			{
				_apiKeyCache.Remove(oldApiKey);
			}
			catch (Exception)
			{
			}
		}

	}
}