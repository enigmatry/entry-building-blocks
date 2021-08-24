using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class PatternValidationRule : ValidationRule
    {
        public PatternValidationRule(PropertyInfo propertyInfo, Regex value, string message, string messageTranslationId)
            : base(
                  "pattern",
                  value,
                  propertyInfo,
                  String.IsNullOrWhiteSpace(message)
                    ? $"{propertyInfo.Name} is not in valid format"
                    : message,
                  messageTranslationId)
        { }

        public override string AsNameValueString() => $"{Name}: {(Regex)Value}";
    }
}
