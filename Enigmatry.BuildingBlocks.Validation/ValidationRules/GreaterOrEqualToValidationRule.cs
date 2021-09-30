using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class GreaterOrEqualToValidationRule<T> : ValidationRule<T>
    {
        public GreaterOrEqualToValidationRule(T value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(value, propertyInfo, expression, String.Empty, "validators.min")
        { }

        public override string FormlyRuleName => "min";

        public override string[] FormlyTemplateOptions =>
            new[]
            {
                "type: 'number'",
                $"{FormlyRuleName}: {Rule}"
            };

        public override string FormlyValidationMessage => HasCustomMessage
            ? CustomMessage
            : "${field?.templateOptions?.label}:property-name: value should be more than ${field?.templateOptions?.min}:min-value:";
    }
}
