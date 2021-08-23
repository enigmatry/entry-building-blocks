using Enigmatry.BuildingBlocks.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Enigmatry.BuildingBlocks.Validation
{
    public class ValidationConfigurationBuilder<T> where T : class
    {
        public IList<PropertyValidationBuilder> PropertyValidations { get; }

        public ValidationConfigurationBuilder()
        {
            PropertyValidations = typeof(T)
                .GetProperties()
                .Select(propertyInfo => new PropertyValidationBuilder(propertyInfo))
                .ToList();
        }

        public PropertyValidationBuilder RuleFor<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var propertyInfo = propertyExpression.GetPropertyInfo();
            var validationRuleBuilder = PropertyValidations.FirstOrDefault(builder => builder.PropertyInfo == propertyInfo);

            if (validationRuleBuilder == null)
            {
                validationRuleBuilder = new PropertyValidationBuilder(propertyInfo);
                PropertyValidations.Add(validationRuleBuilder);
            }

            return validationRuleBuilder;
        }
    }
}
