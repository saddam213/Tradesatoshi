using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Web.Api.Authentication
{
	public class UserApiAuthKey
	{
		public UserApiAuthKey()
		{
		}

		public UserApiAuthKey(string userId, string key, string secret, bool isenabled)
		{
			Key = key;
			UserId = userId;
			Secret = secret;
			IsEnabled = isenabled;
		}

		public string UserId { get; set; }
		public string Secret { get; set; }
		public string Key { get; set; }
		public bool IsEnabled { get; set; }

		public bool IsValid
		{
			get { return !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Secret) && !string.IsNullOrEmpty(Key); }
		}
	}
}