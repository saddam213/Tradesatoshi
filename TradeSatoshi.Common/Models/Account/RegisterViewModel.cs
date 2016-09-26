﻿using System;
using System.ComponentModel.DataAnnotations;
using TradeSatoshi.Common.Validation;
//using TradeSatoshi.Web.Validation;

namespace TradeSatoshi.Common.Account
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

		[Required]
		[Display(Name = "First name")]
		public string FirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[Display(Name = "Date of Birth")]
		public DateTime BirthDate { get; set; }
	}
}