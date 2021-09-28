using Enigmatry.BuildingBlocks.Validation.Helpers;
using Humanizer;
using System;

namespace Enigmatry.BuildingBlocks.Validation.PropertyValidations
{
    public interface IPropertyValidationBuilder<T, TProperty> : IInitialPropertyValidationBuilder<T, TProperty>
    {
        /// <summary>
        /// Adds custom validation message with optional I18N Translation Id
        /// </summary>
        /// <param name="message">Custom validation message</param>
        /// <param name="messageTranlsationId">Custom message I18N translation Id</param>
        /// <returns></returns>
        public IPropertyValidationBuilder<T, TProperty> WithMessage(string message, string messageTranlsationId = "");
    }

    public class PropertyValidationBuilder<T, TProperty> : BasePropertyValidationBuilder<T, TProperty>, IPropertyValidationBuilder<T, TProperty>
    {
        public PropertyValidationBuilder(IPropertyValidation<T, TProperty> propertyRule) : base(propertyRule) { }

        public IPropertyValidationBuilder<T, TProperty> WithMessage(string message, string messageTranlsationId = "")
        {
            if (CurrentValidationRule == null)
            {
                throw new NullReferenceException("Current validation rule not selected");
            }

            Check.IfEmpty(message, $"{CurrentValidationRule.PropertyName.Pascalize()} validation message cannot be empty.");

            CurrentValidationRule.SetCustomMessage(message);

            if (!String.IsNullOrWhiteSpace(messageTranlsationId))
            {
                CurrentValidationRule.SetMessageTranslationId(messageTranlsationId);
            }

            return this;
        }
    }
}
