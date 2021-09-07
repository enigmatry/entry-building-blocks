﻿using Enigmatry.BuildingBlocks.Validation.PropertyValidations;
using Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules;
using Enigmatry.BuildingBlocks.Validation.ValidationRules.CustomValidationRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Enigmatry.BuildingBlocks.Validation
{
    public abstract class ValidationConfiguration<T> : IHasValidationRules where T : class
    {
        private IList<IPropertyValidation<T>> PropertyValidations { get; set; } = new List<IPropertyValidation<T>>();

        public IEnumerable<BuiltInValidationRule> BuiltInValidationRules => PropertyValidations
            .SelectMany(propertyValidation => propertyValidation.Rules)
            .Where(rule => rule.GetType().IsSubclassOf(typeof(BuiltInValidationRule)))
            .Select(rule => (BuiltInValidationRule)rule);

        public IEnumerable<CustomValidatorValidationRule> ValidatorValidationRules => PropertyValidations
            .SelectMany(propertyValidation => propertyValidation.Rules)
            .Where(rule => rule.GetType().IsAssignableFrom(typeof(CustomValidatorValidationRule)))
            .Select(rule => (CustomValidatorValidationRule)rule);

        public IEnumerable<AsyncCustomValidatorValidationRule> AsyncValidatorValidationRules => PropertyValidations
            .SelectMany(propertyValidation => propertyValidation.Rules)
            .Where(rule => rule.GetType().IsAssignableFrom(typeof(AsyncCustomValidatorValidationRule)))
            .Select(rule => (AsyncCustomValidatorValidationRule)rule);

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
