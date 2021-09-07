using Enigmatry.BuildingBlocks.Validation.Helpers;
using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules;
using Enigmatry.BuildingBlocks.Validation.ValidationRules.CustomValidationRules;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation.PropertyValidations
{
    public class InitialPropertyValidationBuilder<T, TProperty>
    {
        protected IPropertyValidation<T, TProperty> PropertyRule { get; set; }
        protected IValidationRule? CurrentValidationRule { get; set; } = null;

        public InitialPropertyValidationBuilder(IPropertyValidation<T, TProperty> propertyRule)
        {
            PropertyRule = propertyRule;
        }

        public string PropertyName => PropertyRule.PropertyInfo.Name;

        public PropertyValidationBuilder<T, TProperty> IsRequired()
        {
            AddOrReplace(new IsRequiredValidationRule(PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        public PropertyValidationBuilder<T, TProperty> Min(int value)
        {
            var isNumber = Extensions.IsNumber(PropertyRule.PropertyInfo.PropertyType);
            AddOrReplace(isNumber
                ? new MinValidationRule(value, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression)
                : new MinLengthValidationRule(value, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        public PropertyValidationBuilder<T, TProperty> Max(int value)
        {
            var isNumber = Extensions.IsNumber(PropertyRule.PropertyInfo.PropertyType);
            AddOrReplace(isNumber
                ? new MaxValidationRule(value, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression)
                : new MaxLengthValidationRule(value, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        public PropertyValidationBuilder<T, TProperty> HasPattern(Regex value)
        {
            AddOrReplace(new PatternValidationRule(value, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        public PropertyValidationBuilder<T, TProperty> IsEmailAddress()
        {
            AddOrReplace(new EmailValidationRule(PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        /// <summary>
        /// Validator name must match implementation with following signature:
        ///     (control: FormControl): boolean
        /// </summary>
        /// <param name="validatorName"></param>
        /// <returns></returns>
        public PropertyValidationBuilder<T, TProperty> HasValidator(string validatorName)
        {
            AddOrReplace(new CustomValidatorValidationRule(validatorName, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        /// <summary>
        /// Validator name must match implementation with following signature:
        ///     (control: FormControl): Promise(boolean)
        /// </summary>
        /// <param name="validatorName"></param>
        /// <returns></returns>
        public PropertyValidationBuilder<T, TProperty> HasAsyncValidator(string validatorName)
        {
            AddOrReplace(new AsyncCustomValidatorValidationRule(validatorName, PropertyRule.PropertyInfo, PropertyRule.PropertyExpression));
            return new PropertyValidationBuilder<T, TProperty>(PropertyRule, CurrentValidationRule);
        }

        private void AddOrReplace(IValidationRule validationRule)
        {
            CurrentValidationRule = validationRule;
            PropertyRule.AddOrReplace(CurrentValidationRule);
        }
    }
}
