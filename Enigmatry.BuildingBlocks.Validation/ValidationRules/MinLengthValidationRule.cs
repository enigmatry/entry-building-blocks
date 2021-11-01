using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class MinLengthValidationRule : ValidationRule<int>
    {
        public MinLengthValidationRule(int value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(value, propertyInfo, expression, String.Empty, "validators.minlength")
        { }

        public override string FormlyRuleName => "minlength";

        public override string FormlyValidationMessage => HasCustomMessage
            ? CustomMessage
            : "${field?.templateOptions?.label}:property-name: should have at least ${field?.templateOptions?.minlength}:min-value: characters";
    }
}
