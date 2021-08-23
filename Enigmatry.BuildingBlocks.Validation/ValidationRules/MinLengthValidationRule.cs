using Enigmatry.BuildingBlocks.Validation.Helpers;
using System;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class MinLengthValidationRule : ValidationRule
    {
        public int Value { get; }

        public MinLengthValidationRule(PropertyInfo propertyInfo, int value, string message, string messageTranslationId)
            : base(
                  Check.IsNumber(propertyInfo.PropertyType) ? "min" : "minLength",
                  propertyInfo,
                  String.IsNullOrWhiteSpace(message)
                    ? $"'{propertyInfo.Name}' min {(Check.IsNumber(propertyInfo.PropertyType) ? "value" : "length")} is {value}"
                    : message,
                  messageTranslationId)
        {
            Value = value;
        }
    }
}
