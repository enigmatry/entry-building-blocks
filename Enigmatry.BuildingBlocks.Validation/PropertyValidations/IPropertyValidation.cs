using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.PropertyValidations
{
    public interface IPropertyValidation<T>
    {
        PropertyInfo PropertyInfo { get; }
        IList<IValidationRule> Rules { get; }
    }

    public interface IPropertyValidation<T, TProperty> : IPropertyValidation<T>
    {
        Expression<Func<T, TProperty>> PropertyExpression { get; }
        void AddRule(IValidationRule rule);
    }
}
