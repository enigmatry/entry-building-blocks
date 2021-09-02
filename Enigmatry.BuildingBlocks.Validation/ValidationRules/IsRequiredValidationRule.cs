using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class IsRequiredValidationRule : AbstractValidationRule<bool>
    {
        public IsRequiredValidationRule(PropertyInfo propertyInfo, LambdaExpression expression)
            : base("required", true, propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} is required");
        }

        public override string AsNameRulePair() => $"{Name}: {(Rule ? "true" : "false")}";
    }
}
