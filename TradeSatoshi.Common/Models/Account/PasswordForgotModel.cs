using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Common.Account
{
	public class PasswordForgotModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}