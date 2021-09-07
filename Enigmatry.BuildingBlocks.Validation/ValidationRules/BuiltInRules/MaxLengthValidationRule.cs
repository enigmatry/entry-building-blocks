using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules
{
    public class MaxLengthValidationRule : BuiltInValidationRule<int>
    {
        public const string RuleName = "maxLength";

        public MaxLengthValidationRule(int value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(RuleName, value, propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} should have less then {value} characters");
        }

        public override string AsNameRulePair() => $"{Name}: {Rule}";
    }
}
