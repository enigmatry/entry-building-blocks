﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class LessThenValidationRule<T> : ValidationRule<T>
    {
        public LessThenValidationRule(T value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(value, propertyInfo, expression, String.Empty, "validators.max")
        { }

        public override string FormlyRuleName => "max";

        public override string[] FormlyTemplateOptions =>
            new[]
            {
                "type: 'number'",
                $"{FormlyRuleName}: {Rule} - 1"
            };

        public override string FormlyValidationMessage => HasCustomMessage
            ? CustomMessage
            : "${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.max}:max-value:";
    }
}
