using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class EmailValidationRule : PatternValidationRule
    {
        public EmailValidationRule(PropertyInfo propertyInfo, string message, string messageTranslationId)
            : base(propertyInfo, new Regex(@"/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/"), message, messageTranslationId)
        { }
    }
}
