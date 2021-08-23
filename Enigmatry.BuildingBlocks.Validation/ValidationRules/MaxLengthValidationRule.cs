using Enigmatry.BuildingBlocks.Validation.Helpers;
using System;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class MaxLengthValidationRule : ValidationRule
    {
        public int Value { get; }

        public MaxLengthValidationRule(PropertyInfo propertyInfo, int value, string message, string messageTranslationId)
            : base(
                  Check.IsNumber(propertyInfo.PropertyType) ? "max" : "maxLength",
                  propertyInfo,
                  String.IsNullOrWhiteSpace(message)
                    ? $"'{propertyInfo.Name}' max {(Check.IsNumber(propertyInfo.PropertyType) ? "value" : "length")} is {value}"
                    : message,
                  messageTranslationId)
        {
            Value = value;
        }
    }
}
