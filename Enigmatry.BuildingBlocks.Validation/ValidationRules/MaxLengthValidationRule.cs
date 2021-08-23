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
                        ? $"{propertyInfo.Name} should be more than {value}"
                        : $"{propertyInfo.Name} should have at least {value} characters"
                    : message,
                  messageTranslationId)
        { }
    }
}
