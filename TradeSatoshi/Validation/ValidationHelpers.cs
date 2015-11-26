using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeSatoshi.Validation
{
	public class ValidationHelpers
	{
		public static bool IsValidEmailAddress(string emailAddress)
		{
			return new System.ComponentModel.DataAnnotations
								.EmailAddressAttribute()
								.IsValid(emailAddress);
		}
	}
}