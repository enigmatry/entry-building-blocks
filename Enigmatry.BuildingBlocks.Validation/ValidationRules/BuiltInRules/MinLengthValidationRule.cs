using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules
{
    public class MinLengthValidationRule : BuiltInValidationRule<int>
    {
        public const string RuleName = "minLength";

        public MinLengthValidationRule(int value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(RuleName, value, propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} should have at least {value} characters");
        }

        public override string AsNameRulePair() => $"{Name}: {Rule}";
    }
}
