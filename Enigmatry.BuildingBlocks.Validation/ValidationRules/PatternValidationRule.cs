﻿using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation.ValidationRules
{
    public class PatternValidationRule : AbstractValidationRule<Regex>
    {
        public const string PatternRuleName = "pattern";

        public PatternValidationRule(Regex value, PropertyInfo propertyInfo, LambdaExpression expression)
            : base(PatternRuleName, value, propertyInfo, expression)
        {
            SetMessage($"{propertyInfo.Name} is not in valid format");
        }

        public override string AsNameRulePair() => $"{Name}: {Rule}";
    }
}
