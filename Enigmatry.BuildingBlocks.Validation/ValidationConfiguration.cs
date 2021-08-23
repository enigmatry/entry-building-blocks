using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Enigmatry.BuildingBlocks.Validation
{
    public abstract class ValidationConfiguration<T> : IHasValidationRules where T : class
    {
        private readonly ValidationConfigurationBuilder<T> _builder = new();

        public PropertyValidationBuilder RuleFor<TProperty>(Expression<Func<T, TProperty>> propertyExpression) =>
            _builder.RuleFor(propertyExpression);

        public IEnumerable<ValidationRule> GetValidationRules() =>
            _builder
                .PropertyValidations
                .SelectMany(x => x.ValidationRules);
    }
}
