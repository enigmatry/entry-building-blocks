using Enigmatry.BuildingBlocks.Validation;
using Enigmatry.BuildingBlocks.Validation.ValidationRules.BuiltInRules;
using Enigmatry.BuildingBlocks.Validation.ValidationRules.CustomValidationRules;
using FluentAssertions;
using Humanizer;
using NUnit.Framework;
using System;
using System.Linq;

namespace Enigmatry.BuildingBlocks.Tests.Validation
{
    [Category("unit")]
    public class ValidationConfigurationFixture
    {
        [Test]
        public void ValidationConfiguration()
        {
            var validationConfiguration = new ValidationModelMockConfiguration();

            validationConfiguration.BuiltInValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.NumberValue).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.BuiltInValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.NumberValue).Camelize())
                .Select(x => x.Name)
                .Should().BeEquivalentTo(IsRequiredValidationRule.RequiredRuleName, MinValidationRule.RuleName, MaxValidationRule.RuleName);

            validationConfiguration.BuiltInValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.TextValue).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.BuiltInValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.TextValue).Camelize())
                .Select(x => x.Name)
                .Should().BeEquivalentTo(IsRequiredValidationRule.RequiredRuleName, MinLengthValidationRule.RuleName, MaxLengthValidationRule.RuleName);

            validationConfiguration.BuiltInValidationRules
                .All(x => !String.IsNullOrWhiteSpace(x.Message))
                .Should().BeTrue("Default messages must be set");

            validationConfiguration.ValidatorValidationRules
                .Should().HaveCount(1);
            validationConfiguration.ValidatorValidationRules
                .All(x => x.PropertyName == nameof(ValidationModelMock.NumberValue).Camelize())
                .Should().BeTrue();
            validationConfiguration.ValidatorValidationRules
                .All(x => !String.IsNullOrWhiteSpace(x.Message))
                .Should().BeTrue("Default messages must be set");

            validationConfiguration.AsyncValidatorValidationRules
                .Should().HaveCount(1);
            validationConfiguration.AsyncValidatorValidationRules
                .All(x => x.PropertyName == nameof(ValidationModelMock.TextValue).Camelize())
                .Should().BeTrue();
            validationConfiguration.AsyncValidatorValidationRules
                .All(x => !String.IsNullOrWhiteSpace(x.Message))
                .Should().BeTrue("Default messages must be set");
        }

        [TestCase(nameof(ValidationModelMock.NumberValue), IsRequiredValidationRule.RequiredRuleName, ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.NumberValue), MinValidationRule.RuleName, ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.NumberValue), MaxValidationRule.RuleName, ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.TextValue), IsRequiredValidationRule.RequiredRuleName, ValidationModelMockConfiguration.CustomValidationMessage, ValidationModelMockConfiguration.CustomValidationMessageTranlsationId)]
        [TestCase(nameof(ValidationModelMock.TextValue), MinLengthValidationRule.RuleName, "TextValue should have at least 2 characters", "")]
        [TestCase(nameof(ValidationModelMock.TextValue), MaxLengthValidationRule.RuleName, "TextValue should have less then 10 characters", "")]
        public void ValidationConfigurationPerValidationRule(
            string propertyName,
            string validationRuleName,
            string validationMessage,
            string validationMessageTranslationId)
        {
            var validationConfiguration = new ValidationModelMockConfiguration();

            validationConfiguration.BuiltInValidationRules
                .Where(x => x.PropertyName == propertyName.Camelize())
                .Should().NotBeNullOrEmpty();
            var validationRule = validationConfiguration.BuiltInValidationRules
                .Where(x => x.PropertyName == propertyName.Camelize())
                .SingleOrDefault(rule => rule.Name == validationRuleName);
            validationRule.Should().NotBeNull();
            validationRule?.Message.Should().Be(validationMessage);
            validationRule?.MessageTranslationId.Should().Be(validationMessageTranslationId);
        }

        [TestCase(nameof(ValidationModelMock.NumberValue), ValidationModelMockConfiguration.ValidatorName, "VALIDATOR_NAME validator condition is not meet", "")]
        public void ValidationConfigurationPerValidatior(
            string propertyName,
            string validatorName,
            string validationMessage,
            string validationMessageTranslationId)
        {
            var validationConfiguration = new ValidationModelMockConfiguration();

            validationConfiguration.ValidatorValidationRules
                .Where(x => x.PropertyName == propertyName.Camelize())
                .Should().NotBeNullOrEmpty();
            var validator = validationConfiguration.ValidatorValidationRules
                .Where(x => x.PropertyName == propertyName.Camelize())
                .SingleOrDefault(rule => rule.ValidatorName == validatorName);
            validator.Should().NotBeNull();
            validator?.Name.Should().Be(CustomValidatorValidationRule.RuleName);
            validator?.Message.Should().Be(validationMessage);
            validator?.MessageTranslationId.Should().Be(validationMessageTranslationId);
        }

        [TestCase(nameof(ValidationModelMock.TextValue),
            ValidationModelMockConfiguration.AsyncValidatorName,
            ValidationModelMockConfiguration.CustomValidationMessage,
            ValidationModelMockConfiguration.CustomValidationMessageTranlsationId)]
        public void ValidationConfigurationPerAsyncValidatior(
            string propertyName,
            string validatorName,
            string validationMessage,
            string validationMessageTranslationId)
        {
            var validationConfiguration = new ValidationModelMockConfiguration();

            validationConfiguration.AsyncValidatorValidationRules
                .Where(x => x.PropertyName == propertyName.Camelize())
                .Should().NotBeNullOrEmpty();
            var asyncValidator = validationConfiguration.AsyncValidatorValidationRules
                .Where(x => x.PropertyName == propertyName.Camelize())
                .SingleOrDefault(rule => rule.ValidatorName == validatorName);
            asyncValidator.Should().NotBeNull();
            asyncValidator?.Name.Should().Be(AsyncCustomValidatorValidationRule.RuleName);
            asyncValidator?.Message.Should().Be(validationMessage);
            asyncValidator?.MessageTranslationId.Should().Be(validationMessageTranslationId);
        }
    }

    public class ValidationModelMock
    {
        public int NumberValue { get; set; }
        public string TextValue { get; set; } = String.Empty;
    }

    public class ValidationModelMockConfiguration : ValidationConfiguration<ValidationModelMock>
    {
        public const string CustomValidationMessage = "CUSTOM_VALIDATION_MESSAGE";
        public const string CustomValidationMessageTranlsationId = "CUSTOM_VALIDATION_MESSAGE_TRANSLATION_ID";
        public const string ValidatorName = "VALIDATOR_NAME";
        public const string AsyncValidatorName = "ASYNC_VALIDATOR_NAME";

        public ValidationModelMockConfiguration()
        {
            RuleFor(x => x.NumberValue)
                .IsRequired()
                    .WithMessage(CustomValidationMessage)
                .Min(2)
                    .WithMessage(CustomValidationMessage)
                .Max(10)
                    .WithMessage(CustomValidationMessage)
                .HasValidator("TO_BE_OVERRIDEN_VALIDATOR")
                .HasValidator(ValidatorName);

            RuleFor(x => x.TextValue)
                .IsRequired()
                    .WithMessage(CustomValidationMessage)
                    .WithMessageTranslationId(CustomValidationMessageTranlsationId)
                .Min(2)
                .Max(10)
                .HasAsyncValidator(AsyncValidatorName)
                    .WithMessage(CustomValidationMessage)
                    .WithMessageTranslationId(CustomValidationMessageTranlsationId);
        }
    }
}
