using Enigmatry.BuildingBlocks.Validation.Helpers;
using Enigmatry.BuildingBlocks.Validation.PropertyValidations;
using Enigmatry.BuildingBlocks.Validation.ValidationRules;
using System;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Validation
{
    public static class InitialPropertyValidationBuilderExtensions
    {
        public static IPropertyMessageValidationBuilder<T, TProperty> IsRequired<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder)
        {
            var response = new PropertyMessageValidationBuilder<T, TProperty>(builder.PropertyRule);
            response.SetValidationRule(new IsRequiredValidationRule(builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyMessageValidationBuilder<T, string> Match<T>(this IInitialPropertyValidationBuilder<T, string> builder, Regex rule)
        {
            var response = new PropertyMessageValidationBuilder<T, string>(builder.PropertyRule);
            response.SetValidationRule(new PatternValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyMessageValidationBuilder<T, string> EmailAddress<T>(this IInitialPropertyValidationBuilder<T, string> builder)
        {
            var response = new PropertyMessageValidationBuilder<T, string>(builder.PropertyRule);
            response.SetValidationRule(new EmailAddressValidationRule(builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyMessageValidationBuilder<T, string> MinLength<T>(this IInitialPropertyValidationBuilder<T, string> builder, int rule)
        {
            var response = new PropertyMessageValidationBuilder<T, string>(builder.PropertyRule);
            response.SetValidationRule(new MinLengthValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyMessageValidationBuilder<T, string> MaxLength<T>(this IInitialPropertyValidationBuilder<T, string> builder, int rule)
        {
            var response = new PropertyMessageValidationBuilder<T, string>(builder.PropertyRule);
            response.SetValidationRule(new MaxLengthValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyMessageValidationBuilder<T, string> Length<T>(this IInitialPropertyValidationBuilder<T, string> builder, int rule)
        {
            var response = MinLength(builder, rule);
            MaxLength(response, rule);
            return response;
        }

        public static IPropertyMessageValidationBuilder<T, TProperty> GreaterOrEqualTo<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty>
        {
            Check.IsNumber(typeof(TProperty));
            var response = new PropertyMessageValidationBuilder<T, TProperty>(builder.PropertyRule);
            response.SetValidationRule(new MinValueValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyMessageValidationBuilder<T, TProperty> GreaterThen<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty> =>
            GreaterOrEqualTo(builder, ++rule);

        public static IPropertyMessageValidationBuilder<T, TProperty> LessOrEqualTo<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty>
        {
            Check.IsNumber(typeof(TProperty));
            var response = new PropertyMessageValidationBuilder<T, TProperty>(builder.PropertyRule);
            response.SetValidationRule(new MaxValueValidationRule(rule, builder.PropertyRule.PropertyInfo, builder.PropertyRule.PropertyExpression));
            return response;
        }

        public static IPropertyMessageValidationBuilder<T, TProperty> LessThen<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty> =>
            LessOrEqualTo(builder, --rule);

        public static IPropertyMessageValidationBuilder<T, TProperty> EqualTo<T, TProperty>(this IInitialPropertyValidationBuilder<T, TProperty> builder, int rule)
            where TProperty : struct, IComparable, IComparable<TProperty>
        {
            Check.IsNumber(typeof(TProperty));
            var response = GreaterOrEqualTo(builder, rule);
            LessOrEqualTo(response, rule);
            return response;
        }
    }
}
