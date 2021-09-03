using Enigmatry.BuildingBlocks.Validation.PropertyValidations;
using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Enigmatry.BuildingBlocks.Validation
{
    public abstract class AbstractValidationConfiguration<T> : IHasValidationRules where T : class
    {
        public IList<IPropertyValidation<T>> PropertyValidations { get; private set; } = new List<IPropertyValidation<T>>();

        public IEnumerable<IValidationRule> ValidationRules => PropertyValidations.SelectMany(x => x.Rules);

        public InitialPropertyValidationBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var propertyValidator = new PropertyValidation<T, TProperty>(propertyExpression);
            AddOrUpdate(propertyValidator);
            return new InitialPropertyValidationBuilder<T, TProperty>(propertyValidator);
        }

        private void AddOrUpdate(IPropertyValidation<T> propertyValidator)
        {
            var existing = PropertyValidations.SingleOrDefault(x => x.PropertyInfo.Name == propertyValidator.PropertyInfo.Name);
            if (existing == null)
            {
                PropertyValidations.Add(propertyValidator);
            }
            else
            {
                existing = propertyValidator;
            }
        }
    }
}
