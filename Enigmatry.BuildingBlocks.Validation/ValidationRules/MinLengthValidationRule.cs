using Enigmatry.BuildingBlocks.Validation.Helpers;
using System;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class MinLengthValidationRule : ValidationRule
    {
        public MinLengthValidationRule(PropertyInfo propertyInfo, int value, string message, string messageTranslationId)
            : base(
                  Check.IsNumber(propertyInfo.PropertyType) ? "min" : "minLength",
                  value,
                  propertyInfo,
                  String.IsNullOrWhiteSpace(message)
                    ? Check.IsNumber(propertyInfo.PropertyType)
                        ? $"{propertyInfo.Name} should be less then {value}"
                        : $"{propertyInfo.Name} should be less then {value} characters"
                    : message,
                  messageTranslationId)
        { }
    }
}
