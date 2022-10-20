using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.Entry.Validation.ValidationRules
{
    public class EmailAddressValidationRule : PatternValidationRule
    {
        public EmailAddressValidationRule(PropertyInfo propertyInfo, LambdaExpression expression)
            : base(
                  new Regex(@"/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/"),
                  propertyInfo,
                  expression,
                  "Invalid email address format",
                  "validators.pattern.emailAddress")
        { }

        public override string FormlyValidationMessage => HasCustomMessage
            ? CustomMessage
            : String.Empty;
    }
}
