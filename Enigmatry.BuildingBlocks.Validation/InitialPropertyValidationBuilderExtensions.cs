using Enigmatry.BuildingBlocks.Validation.Helpers;
using Enigmatry.BuildingBlocks.Validation.PropertyValidations;
using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation
{
    public static class InitialPropertyValidationBuilderExtensions
    {
        public static IPropertyValidationBuilder<T, TProperty> IsRequired<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder)
        {
            var response = new PropertyValidationBuilder<T, TProperty>(builder.PropertyRule);
            response.SetValidationRule(new IsRequiredValidationRule(builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyValidationBuilder<T, string> Match<T>(this IInitialPropertyValidationBuilder<T, string> builder, Regex rule)
        {
            var response = new PropertyValidationBuilder<T, string>(builder.PropertyRule);
            response.SetValidationRule(new PatternValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyValidationBuilder<T, string> EmailAddress<T>(this IInitialPropertyValidationBuilder<T, string> builder)
        {
            var response = new PropertyValidationBuilder<T, string>(builder.PropertyRule);
            response.SetValidationRule(new EmailAddressValidationRule(builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyValidationBuilder<T, string> MinLength<T>(this IInitialPropertyValidationBuilder<T, string> builder, int rule)
        {
            var response = new PropertyValidationBuilder<T, string>(builder.PropertyRule);
            response.SetValidationRule(new MinLengthValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyValidationBuilder<T, string> MaxLength<T>(this IInitialPropertyValidationBuilder<T, string> builder, int rule)
        {
            var response = new PropertyValidationBuilder<T, string>(builder.PropertyRule);
            response.SetValidationRule(new MaxLengthValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyValidationBuilder<T, string> Length<T>(this IInitialPropertyValidationBuilder<T, string> builder, int rule)
        {
            var response = MinLength(builder, rule);
            MaxLength(response, rule);
            return response;
        }

        public static IPropertyValidationBuilder<T, TProperty> GreaterOrEqualTo<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty>
        {
            Check.IfNotNumber(typeof(TProperty));
            var response = new PropertyValidationBuilder<T, TProperty>(builder.PropertyRule);
            response.SetValidationRule(new MinValueValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyValidationBuilder<T, TProperty> GreaterThen<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty> =>
            GreaterOrEqualTo(builder, ++rule);

        public static IPropertyValidationBuilder<T, TProperty> LessOrEqualTo<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty>
        {
            Check.IfNotNumber(typeof(TProperty));
            var response = new PropertyValidationBuilder<T, TProperty>(builder.PropertyRule);
            response.SetValidationRule(new MaxValueValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyValidationBuilder<T, TProperty> LessThen<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty> =>
            LessOrEqualTo(builder, --rule);

        public static IPropertyValidationBuilder<T, TProperty> EqualTo<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty>
        {
            Check.IfNotNumber(typeof(TProperty));
            var response = GreaterOrEqualTo(builder, rule);
            LessOrEqualTo(response, rule);
            return response;
        }
    }
}
