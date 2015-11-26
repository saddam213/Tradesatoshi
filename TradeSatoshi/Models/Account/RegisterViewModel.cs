using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TradeSatoshi.Validation;

namespace TradeSatoshi.Models.Account
{
	public class RegisterViewModel
	{
		[Required]
		[RegularExpression(@"^\w+$", ErrorMessage = @"UserName can only contain letters, digits and underscore")]
		public string UserName { get; set; }

		[Required]
		[Display(Name = "Email Address")]
		[EmailAddress]
		public string EmailAddress { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[RequiredToBeTrue(ErrorMessage = "You must accept the terms and conditions to register.")]
		public bool AcceptTerms { get; set; }

		[RequiredToBeTrue(ErrorMessage = "You must be at least 18 years of age to register.")]
		[Display(Name = "I am older that 18 years of age.")]
		public bool AgeRestriction { get; set; }
	}
}