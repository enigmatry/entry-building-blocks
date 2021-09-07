using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules
{
    public class MinValidationRule : BuiltInValidationRule<int>
    {
        public const string RuleName = "min";

        public MinValidationRule(int value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(RuleName, value, propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} should be more then {value}");
        }

        public override string AsNameRulePair() => $"{Name}: {Rule}";
    }
}
