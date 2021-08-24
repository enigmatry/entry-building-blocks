using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation
{
    public class PropertyValidationBuilder
    {
        public IList<ValidationRule> ValidationRules { get; }
        public PropertyInfo PropertyInfo { get; }

        public PropertyValidationBuilder(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            ValidationRules = new List<ValidationRule>();
        }

        public PropertyValidationBuilder IsRequired(string message = "", string messageTranslationId = "")
        {
            AddOrUpdateRule(new IsRequiredValidationRule(PropertyInfo, message, messageTranslationId));
            return this;
        }

        public PropertyValidationBuilder HasMinLength(int value, string message = "", string messageTranslationId = "")
        {
            AddOrUpdateRule(new MinLengthValidationRule(PropertyInfo, value, message, messageTranslationId));
            return this;
        }

        public PropertyValidationBuilder HasMaxLength(int value, string message = "", string messageTranslationId = "")
        {
            AddOrUpdateRule(new MaxLengthValidationRule(PropertyInfo, value, message, messageTranslationId));
            return this;
        }

        public PropertyValidationBuilder HasPattern(Regex value, string message = "", string messageTranslationId = "")
        {
            AddOrUpdateRule(new PatternValidationRule(PropertyInfo, value, message, messageTranslationId));
            return this;
        }

        public PropertyValidationBuilder IsEmail(string message = "", string messageTranslationId = "")
        {
            AddOrUpdateRule(new EmailValidationRule(PropertyInfo, message, messageTranslationId));
            return this;
        }

        public PropertyValidationBuilder Build => new(PropertyInfo);

        private void AddOrUpdateRule(ValidationRule rule)
        {
            var existing = ValidationRules.SingleOrDefault(x => x.Name == rule.Name);
            if (existing == null)
            {
                ValidationRules.Add(rule);
            }
            else
            {
                existing = rule;
            }
        }
    }
}
