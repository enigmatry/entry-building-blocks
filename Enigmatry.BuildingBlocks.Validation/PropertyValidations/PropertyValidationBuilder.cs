using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System;

namespace Enigmatry.BuildingBlocks.Validation.PropertyValidations
{
    public class PropertyValidationBuilder<T, TProperty> : InitPropertyValidationBuilder<T, TProperty>
    {
        public PropertyValidationBuilder(IPropertyValidation<T, TProperty> propertyRule, IValidationRule? currentValidationRule)
            : base(propertyRule)
        {
            CurrentValidationRule = currentValidationRule;
        }

        public PropertyValidationBuilder<T, TProperty> WithMessage(string message)
        {
            if (CurrentValidationRule == null)
            {
                throw new NullReferenceException("Current validation rule not selected");
            }
            CurrentValidationRule.SetMessage(message);
            return this;
        }

        public PropertyValidationBuilder<T, TProperty> WithMessageTranslationId(string messageTranslationId)
        {
            if (CurrentValidationRule == null)
            {
                throw new NullReferenceException("Current validation rule not selected");
            }
            CurrentValidationRule.SetMessageTranslationId(messageTranslationId);
            return this;
        }
    }
}
