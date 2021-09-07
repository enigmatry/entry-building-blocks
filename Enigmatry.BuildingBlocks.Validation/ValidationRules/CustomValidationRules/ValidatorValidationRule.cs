using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.CustomValidationRules
{
    public abstract class ValidatorValidationRule : ValidationRule
    {
        public string ValidatorName { get; private set; } = String.Empty;

        protected ValidatorValidationRule(string validatorName, string name, LambdaExpression expression, PropertyInfo propertyInfo)
            : base(name, expression, propertyInfo)
        {
            ValidatorName = validatorName;
        }
    }
}
