using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class EmailValidationRule : PatternValidationRule
    {
        public EmailValidationRule(PropertyInfo propertyInfo, LambdaExpression expression)
            : base(new Regex(@"/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/"), propertyInfo, expression)
        {
            SetMessage($"{nameof(propertyInfo.Name)} not in correct email address format");
        }
    }
}
