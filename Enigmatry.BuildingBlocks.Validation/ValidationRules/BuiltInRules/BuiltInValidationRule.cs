using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules
{
    public abstract class BuiltInValidationRule : ValidationRule
    {
        protected BuiltInValidationRule(string name, LambdaExpression expression, PropertyInfo propertyInfo)
            : base(name, expression, propertyInfo)
        { }

        public abstract string AsNameRulePair();
    }

    public abstract class BuiltInValidationRule<TRule> : BuiltInValidationRule
    {
        public TRule Rule { get; }

        protected BuiltInValidationRule(string name, TRule rule, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(name, expression, propertyInfo)
        {
            Rule = rule;
        }
    }
}
