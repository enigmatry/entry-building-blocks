using System;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class IsRequiredValidationRule : ValidationRule
    {
        public IsRequiredValidationRule(PropertyInfo propertyInfo, string message, string messageTranslationId)
            : base(
                  "required",
                  propertyInfo,
                  String.IsNullOrWhiteSpace(message)
                    ? $"'{propertyInfo.Name}' is required"
                    : message,
                  messageTranslationId) { }
    }
}
