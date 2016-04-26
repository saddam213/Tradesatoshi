using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Web.Api.Authentication
{
	public class ApiKeyStore
	{
		private static ConcurrentDictionary<string, UserApiAuthKey> _apiAuthKeys = new ConcurrentDictionary<string, UserApiAuthKey>();

		private static ConcurrentDictionary<string, UserApiAuthKey> ApiAuthKeys
		{
			get
			{
				if (_apiAuthKeys == null || _apiAuthKeys.Count <= 0)
				{
					//using (var context = new ApplicationDbContext())
					//{
					//	var users = context.Users.Where(x => x.IsApiEnabled && !string.IsNullOrEmpty(x.ApiKey));
					//	if (users != null)
					//	{
					//		foreach (var user in users)
					//		{
					//			_apiAuthKeys.TryAdd(user.ApiKey, new UserApiAuthKey
					//			{
					//				Key = user.ApiKey,
					//				Secret = user.ApiSecret,
					//				UserId = user.Id
					//			});
					//		}
					//	}
					//}
				}
				return _apiAuthKeys;
			}
		}

		//public static bool UpdateApiAuthKey(string userId, UpdateApiModel newKey)
		//{
		//	UserApiAuthKey authKey = null;
		//	if (ApiAuthKeys.Values.Any(x => x.UserId == userId))
		//	{
		//		var key = ApiAuthKeys.FirstOrDefault(x => x.Value.UserId == userId);
		//		ApiAuthKeys.TryRemove(key.Key, out authKey);
		//	}

		//	if (!newKey.IsApiEnabled)
		//	{
		//		return true;
		//	}

		//	return _apiAuthKeys.TryAdd(newKey.ApiKey, new UserApiAuthKey
		//	{
		//		Key = newKey.ApiKey,
		//		Secret = newKey.ApiSecret,
		//		UserId = userId
		//	});
		//}

		public static UserApiAuthKey GetApiAuthKey(string apiKey)
		{
			UserApiAuthKey authKey = null;
			if (!ApiAuthKeys.TryGetValue(apiKey, out authKey))
			{
			}
			return authKey;
		}
	}
}