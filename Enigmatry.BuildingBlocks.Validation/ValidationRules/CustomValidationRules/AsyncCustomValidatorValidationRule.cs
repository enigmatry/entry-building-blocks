using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.CustomValidationRules
{
    public class AsyncCustomValidatorValidationRule : ValidatorValidationRule
    {
        public const string RuleName = "custom-async-validator";

        public AsyncCustomValidatorValidationRule(string validatorName, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(validatorName, RuleName, expression, propertyInfo)
        {
            SetMessage($"{validatorName} validator condition is not meet");
        }
    }
}
