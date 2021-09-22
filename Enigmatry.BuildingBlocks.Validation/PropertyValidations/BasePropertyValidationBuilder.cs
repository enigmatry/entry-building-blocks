using Enigmatry.BuildingBlocks.Validation.ValidationRules;

namespace Enigmatry.BuildingBlocks.Validation.PropertyValidations
{
    public interface IBasePropertyValidationBuilder<T, TProperty>
    {
        IPropertyValidation<T, TProperty> PropertyRule { get; }
        void SetValidationRule(IValidationRule validationRule);
    }

    public abstract class BasePropertyValidationBuilder<T, TProperty> : IBasePropertyValidationBuilder<T, TProperty>
    {
        public IPropertyValidation<T, TProperty> PropertyRule { get; private set; }
        protected IValidationRule? CurrentValidationRule { get; set; } = null;

        public BasePropertyValidationBuilder(IPropertyValidation<T, TProperty> propertyRule)
        {
            PropertyRule = propertyRule;
        }

        public void SetValidationRule(IValidationRule validationRule)
        {
            CurrentValidationRule = validationRule;
            PropertyRule.AddOrReplace(CurrentValidationRule);
        }
    }
}
