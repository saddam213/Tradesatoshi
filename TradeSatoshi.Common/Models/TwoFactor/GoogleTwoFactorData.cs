using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.TwoFactor
{
	public class GoogleTwoFactorData
	{
		public string PrivateKey { get; set; }
		public string PublicKey { get; set; }
		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(PublicKey)
				  && !string.IsNullOrEmpty(PrivateKey);
			}
		}
	}
}
