using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.Entry.Validation.ValidationRules
{
    public class PatternValidationRule : ValidationRule<Regex>
    {
        public PatternValidationRule(
            Regex value,
            PropertyInfo propertyInfo,
            LambdaExpression expression,
            string message = "",
            string messageTranslationId = "validators.pattern")
            : base(value, propertyInfo, expression, message, messageTranslationId)
        { }

        public override string FormlyRuleName => "pattern";

        public override string FormlyValidationMessage => HasCustomMessage
            ? CustomMessage
            : "${field?.templateOptions?.label}:property-name: is not in valid format";
    }
}
