
namespace TradeSatoshi.Common.Validation
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