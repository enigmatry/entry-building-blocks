using Enigmatry.BuildingBlocks.Validation.Helpers;
using System;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class MaxLengthValidationRule : ValidationRule
    {
        public MaxLengthValidationRule(PropertyInfo propertyInfo, int value, string message, string messageTranslationId)
            : base(
                  Check.IsNumber(propertyInfo.PropertyType) ? "max" : "maxLength",
                  value,
                  propertyInfo,
                  String.IsNullOrWhiteSpace(message)
                    ? Check.IsNumber(propertyInfo.PropertyType)
                        ? $"{propertyInfo.Name} should be less than {value}"
                        : $"{propertyInfo.Name} should have less then {value} characters"
                    : message,
                  messageTranslationId)
        { }

        public override string AsNameValueString() => $"{Name}: {Value}";
    }
}
