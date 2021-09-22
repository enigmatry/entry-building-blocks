using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class IsRequiredValidationRule : ValidationRule<bool>
    {
        public IsRequiredValidationRule(PropertyInfo propertyInfo, LambdaExpression expression)
            : base(true, propertyInfo, expression, String.Empty, "validators.required")
        { }

        public override string FormlyRuleName => "required";

        public override string[] FormlyTemplateOptions =>
            new[] { $"{FormlyRuleName}: {Rule.ToString().ToLowerInvariant()}" };

        public override string FormlyValidationMessage => HasCustomMessage
            ? CustomMessage
            : "${field?.templateOptions?.label}:property-name: is required";
    }
}
