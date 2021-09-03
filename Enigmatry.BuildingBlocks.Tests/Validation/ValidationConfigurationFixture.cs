using Enigmatry.BuildingBlocks.Validation;
using Enigmatry.BuildingBlocks.Validation.ValidationRules;
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

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.NumberValue).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.NumberValue).Camelize())
                .Select(x => x.Name)
                .Should().BeEquivalentTo(IsRequiredValidationRule.RequiredRuleName, MinValidationRule.MinValueRuleName, MaxValidationRule.MaxValueRuleName);

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.TextValue).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.TextValue).Camelize())
                .Select(x => x.Name)
                .Should().BeEquivalentTo(IsRequiredValidationRule.RequiredRuleName, MinValidationRule.MinLengthRuleName, MaxValidationRule.MaxLengthRuleName);

            validationConfiguration.ValidationRules
                .All(x => !String.IsNullOrWhiteSpace(x.Message))
                .Should().BeTrue("Default messages must be set");
        }

        [TestCase(nameof(ValidationModelMock.NumberValue), IsRequiredValidationRule.RequiredRuleName, ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.NumberValue), MinValidationRule.MinValueRuleName, ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.NumberValue), MaxValidationRule.MaxValueRuleName, ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.TextValue), IsRequiredValidationRule.RequiredRuleName, ValidationModelMockConfiguration.CustomValidationMessage, ValidationModelMockConfiguration.CustomValidationMessageTranlsationId)]
        [TestCase(nameof(ValidationModelMock.TextValue), MinValidationRule.MinLengthRuleName, "TextValue should have at least 2 characters", "")]
        [TestCase(nameof(ValidationModelMock.TextValue), MaxValidationRule.MaxLengthRuleName, "TextValue should have less then 10 characters", "")]
        public void ValidationConfigurationPerValidator(string propertyName,
                                                        string validationRuleName,
                                                        string validationMessage,
                                                        string validationMessageTranslationId)
        {
            var validationConfiguration = new ValidationModelMockConfiguration();

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == propertyName.Camelize())
                .Should().NotBeNullOrEmpty();
            var validationRule = validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == propertyName.Camelize())
                .SingleOrDefault(rule => rule.Name == validationRuleName);
            validationRule.Should().NotBeNull();
            validationRule?.Message.Should().Be(validationMessage);
            validationRule?.MessageTranslationId.Should().Be(validationMessageTranslationId);
        }
    }

    public class ValidationModelMock
    {
        public int NumberValue { get; set; }
        public string TextValue { get; set; } = String.Empty;
    }

    public class ValidationModelMockConfiguration : AbstractValidationConfiguration<ValidationModelMock>
    {
        public const string CustomValidationMessage = "CUSTOM_VALIDATION_MESSAGE";
        public const string CustomValidationMessageTranlsationId = "CUSTOM_VALIDATION_MESSAGE_TRANSLATION_ID";

        public ValidationModelMockConfiguration()
        {
            RuleFor(x => x.NumberValue)
                .IsRequired()
                    .WithMessage(CustomValidationMessage)
                .Min(2)
                    .WithMessage(CustomValidationMessage)
                .Max(10)
                    .WithMessage(CustomValidationMessage);

            RuleFor(x => x.TextValue)
                .IsRequired()
                    .WithMessage(CustomValidationMessage)
                    .WithMessageTranslationId(CustomValidationMessageTranlsationId)
                .Min(2)
                .Max(10);
        }
    }
}
