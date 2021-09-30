﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class GreaterThenValidationRule<T> : ValidationRule<T>
    {
        public GreaterThenValidationRule(T value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(value, propertyInfo, expression, String.Empty, "validators.min")
        { }

        public override string FormlyRuleName => "min";

        public override string[] FormlyTemplateOptions =>
            new[]
            {
                "type: 'number'",
                $"{FormlyRuleName}: {Rule} + 1"
            };

        public override string FormlyValidationMessage => HasCustomMessage
            ? CustomMessage
            : "${field?.templateOptions?.label}:property-name: value should be more than ${field?.templateOptions?.min}:min-value:";
    }
}
