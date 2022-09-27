using Enigmatry.Entry.Validation.Helpers;
using Enigmatry.Entry.Validation.ValidationRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.Entry.Validation.PropertyValidations
{
    public class PropertyValidation<T, TProperty> : IPropertyValidation<T, TProperty>
    {
        public Expression<Func<T, TProperty>> PropertyExpression { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public IList<IValidationRule> Rules { get; private set; }

        public PropertyValidation(Expression<Func<T, TProperty>> propertyExpression)
        {
            PropertyExpression = propertyExpression;
            PropertyInfo = PropertyExpression.TryGetProperty()
                ?? throw new InvalidOperationException($"{nameof(PropertyInfo)} could not be extracted from {nameof(propertyExpression)}");
            Rules = new List<IValidationRule>();
        }

        public void AddOrReplace(IValidationRule rule)
        {
            var existing = Rules.SingleOrDefault(x => x.FormlyRuleName == rule.FormlyRuleName);

            if (existing != null)
            {
                Rules.Remove(existing);
            }
            Rules.Add(rule);
        }
    }
}
