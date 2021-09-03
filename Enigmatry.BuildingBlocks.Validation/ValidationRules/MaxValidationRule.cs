using Enigmatry.BuildingBlocks.Validation.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class MaxValidationRule : AbstractValidationRule<int>
    {
        public const string MaxValueRuleName = "max";
        public const string MaxLengthRuleName = "maxLength";

        public MaxValidationRule(int value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(Extensions.IsNumber(propertyInfo.PropertyType) ? MaxValueRuleName : MaxLengthRuleName, value, propertyInfo, expression)
        {
            SetMessage(Extensions.IsNumber(propertyInfo.PropertyType)
                ? $"{propertyInfo.Name} should be less than {value}"
                : $"{propertyInfo.Name} should have less then {value} characters");
        }

        public override string AsNameRulePair() => $"{Name}: {Rule}";
    }
}
