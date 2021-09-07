using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules
{
    public class PatternValidationRule : BuiltInValidationRule<Regex>
    {
        public const string RuleName = "pattern";

        public PatternValidationRule(Regex value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(RuleName, value, propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} is not in valid format");
        }

        public override string AsNameRulePair() => $"{Name}: {Rule}";
    }
}
