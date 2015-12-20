using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.TwoFactor;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.TwoFactor
{
	public class CreateTwoFactorModel
	{
		public TwoFactorType Type { get; set; }
		public TwoFactorComponentType ComponentType { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email Address")]
		public string DataEmail { get; set; }

		[Required]
		[Display(Name = "Pin Code")]
		[RegularExpression(@"^\d{4,8}$", ErrorMessage = "The pin must be between 4 and 8 digits.")]
		public string DataPin { get; set; }

		public GoogleTwoFactorData GoogleData { get; set; }

		public string Data
		{
			get
			{
				switch (Type)
				{
					case TwoFactorType.EmailCode:
						return DataEmail;
					case TwoFactorType.GoogleCode:
						return GoogleData.PrivateKey;
					case TwoFactorType.PinCode:
						return DataPin;
					default:
						break;
				}
				return string.Empty;
			}
		}

		public bool IsValid(ModelStateDictionary modelState)
		{
			modelState.Clear();
			if (Type == TwoFactorType.None)
			{
				modelState.AddModelError("", "Please select a TFA option");
				return false;
			}

			if (Type == TwoFactorType.EmailCode)
			{
				if (string.IsNullOrEmpty(DataEmail) || !ValidationHelpers.IsValidEmailAddress(DataEmail))
				{
					modelState.AddModelError("", string.Format("'{0}' is not a valid email address.", DataEmail));
					return false;
				}
			}
			if (Type == TwoFactorType.GoogleCode)
			{
				if (!GoogleData.IsValid)
				{
					modelState.AddModelError("", "An Unknown error occured.");
					return false;
				}
			}
			if (Type == TwoFactorType.PinCode)
			{
				if (string.IsNullOrEmpty(DataPin) || DataPin.Length < 4 || DataPin.Length > 8 || !DataPin.All(char.IsDigit))
				{
					modelState.AddModelError("", "The pin must be between 4 and 8 digits.");
					return false;
				}
			}
			return true;
		}
	}
}