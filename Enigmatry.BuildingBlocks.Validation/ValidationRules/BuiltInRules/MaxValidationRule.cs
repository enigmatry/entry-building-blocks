using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules
{
    public class MaxValidationRule : BuiltInValidationRule<int>
    {
        public const string RuleName = "max";

        public MaxValidationRule(int value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(RuleName, value, propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} should be less than {value}");
        }

        public override string AsNameRulePair() => $"{Name}: {Rule}";
    }
}
