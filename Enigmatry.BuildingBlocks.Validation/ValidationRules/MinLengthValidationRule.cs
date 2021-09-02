using Enigmatry.BuildingBlocks.Validation.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class MinLengthValidationRule : AbstractValidationRule<int>
    {
        public MinLengthValidationRule(int value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(Extensions.IsNumber(propertyInfo.PropertyType) ? "min" : "minLength", value, propertyInfo, expression)
        {
            SetMessage(Extensions.IsNumber(propertyInfo.PropertyType)
                ? $"{propertyInfo.Name} should be more then {value}"
                : $"{propertyInfo.Name} should have at least {value} characters");
        }

        public override string AsNameRulePair() => $"{Name}: {Rule}";
    }
}
