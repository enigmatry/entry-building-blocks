using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules
{
    public class IsRequiredValidationRule : BuiltInValidationRule<bool>
    {
        public const string RequiredRuleName = "required";

        public IsRequiredValidationRule(PropertyInfo propertyInfo, LambdaExpression expression)
            : base(RequiredRuleName, true, propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} is required");
        }

        public override string AsNameRulePair() => $"{Name}: {(Rule ? "true" : "false")}";
    }
}
