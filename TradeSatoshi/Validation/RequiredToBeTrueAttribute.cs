using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradeSatoshi.Validation
{
	public class RequiredToBeTrueAttribute : RequiredAttribute
	{
		public override bool IsValid(object value)
		{
			if (value is bool)
				return (bool)value;
			else
				return true;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
			ModelMetadata metadata,
			ControllerContext context)
		{
			yield return new ModelClientValidationRule
			{
				ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
				ValidationType = "booleanrequired"
			};
		}

	}
}