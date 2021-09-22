using Enigmatry.BuildingBlocks.Validation.Helpers;
using Humanizer;
using System;

namespace Enigmatry.BuildingBlocks.Validation.PropertyValidations
{
    public interface IPropertyMessageValidationBuilder<T, TProperty> : IInitialPropertyValidationBuilder<T, TProperty>
    {
        public IPropertyMessageValidationBuilder<T, TProperty> WithMessage(string message, string messageTranlsationId = "");
    }

    public class PropertyMessageValidationBuilder<T, TProperty> : BasePropertyValidationBuilder<T, TProperty>, IPropertyMessageValidationBuilder<T, TProperty>
    {
        public PropertyMessageValidationBuilder(IPropertyValidation<T, TProperty> propertyRule) : base(propertyRule) { }

        public IPropertyMessageValidationBuilder<T, TProperty> WithMessage(string message, string messageTranlsationId = "")
        {
            if (CurrentValidationRule == null)
            {
                throw new NullReferenceException("Current validation rule not selected");
            }

            Check.IsEmpty(message, $"{CurrentValidationRule.PropertyName.Pascalize()} validation message cannot be empty.");

            CurrentValidationRule.SetCustomMessage(message);

            if (!String.IsNullOrWhiteSpace(messageTranlsationId))
            {
                CurrentValidationRule.SetMessageTranslationId(messageTranlsationId);
            }

            return this;
        }
    }
}
