namespace Enigmatry.BuildingBlocks.Validation.PropertyValidations
{
    public interface IInitialPropertyValidationBuilder<T, TProperty> : IBasePropertyValidationBuilder<T, TProperty> { }

    public class InitialPropertyValidationBuilder<T, TProperty> : BasePropertyValidationBuilder<T, TProperty>, IInitialPropertyValidationBuilder<T, TProperty>
    {
        public InitialPropertyValidationBuilder(IPropertyValidation<T, TProperty> propertyRule) : base(propertyRule) { }
    }
}
