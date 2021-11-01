using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class MaxLengthValidationRule : ValidationRule<int>
    {
        public MaxLengthValidationRule(int value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(value, propertyInfo, expression, String.Empty, "validators.maxlength")
        { }

        public override string FormlyRuleName => "maxlength";

        public override string FormlyValidationMessage => HasCustomMessage
            ? CustomMessage
            : "${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.maxlength}:max-value: characters";
    }
}
