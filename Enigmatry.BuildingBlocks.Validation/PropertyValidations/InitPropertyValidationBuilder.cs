﻿using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System.Linq;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation.PropertyValidations
{
    public class InitPropertyValidationBuilder<T, TProperty>
    {
        protected IPropertyValidation<T, TProperty> PropertyRule { get; set; }
        protected IValidationRule? CurrentValidationRule { get; set; } = null;

        public InitPropertyValidationBuilder(IPropertyValidation<T, TProperty> propertyRule)
        {
            PropertyRule = propertyRule;
        }

        public string PropertyName => PropertyRule.PropertyInfo.Name;

        public PropertyValidationBuilder<T, TProperty> IsRequired()
        {
            AddOrUpdateRule(new IsRequiredValidationRule(PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        public PropertyValidationBuilder<T, TProperty> HasMinLength(int value)
        {
            AddOrUpdateRule(new MinLengthValidationRule(value, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        public PropertyValidationBuilder<T, TProperty> HasMaxLength(int value)
        {
            AddOrUpdateRule(new MaxLengthValidationRule(value, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        public PropertyValidationBuilder<T, TProperty> HasPattern(Regex value)
        {
            AddOrUpdateRule(new PatternValidationRule(value, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        public PropertyValidationBuilder<T, TProperty> IsEmailAddress()
        {
            AddOrUpdateRule(new EmailValidationRule(PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        private void AddOrUpdateRule(IValidationRule rule)
        {
            CurrentValidationRule = rule;
            var existing = PropertyRule.Rules.SingleOrDefault(x => x.Name == rule.Name);
            if (existing == null)
            {
                PropertyRule.AddRule(rule);
            }
            else
            {
                existing = rule;
            }
        }
    }
}
