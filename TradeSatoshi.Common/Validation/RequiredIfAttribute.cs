using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Common.Validation
{
	public class RequiredIfAttribute : RequiredAttribute
	{
		private string PropertyName { get; }
		private object DesiredValue { get; }

		public RequiredIfAttribute(string propertyName, object desiredvalue)
		{
			PropertyName = propertyName;
			DesiredValue = desiredvalue;
		}

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			var instance = context.ObjectInstance;
			var type = instance.GetType();
			var proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
			if (proprtyvalue.ToString() == DesiredValue.ToString())
			{
				var result = base.IsValid(value, context);
				return result;
			}
			return ValidationResult.Success;
		}
	}
}