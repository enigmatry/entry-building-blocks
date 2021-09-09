using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules
{
    public class EmailValidationRule : PatternValidationRule
    {
        public EmailValidationRule(PropertyInfo propertyInfo, LambdaExpression expression)
            : base(new Regex(@"/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/"), propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} is not in correct email address format");
        }
    }
}
