using Enigmatry.BuildingBlocks.Validation;
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
                .Should().BeEquivalentTo("required", "min", "max");

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.TextValue).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.TextValue).Camelize())
                .Select(x => x.Name)
                .Should().BeEquivalentTo("required", "minLength", "maxLength");

            validationConfiguration.ValidationRules
                .All(x => !String.IsNullOrWhiteSpace(x.Message))
                .Should().BeTrue("Default messages must be set");
        }

        [TestCase(nameof(ValidationModelMock.NumberValue), "required", ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.NumberValue), "min", ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.NumberValue), "max", ValidationModelMockConfiguration.CustomValidationMessage, "")]
        [TestCase(nameof(ValidationModelMock.TextValue), "required", ValidationModelMockConfiguration.CustomValidationMessage, ValidationModelMockConfiguration.CustomValidationMessageTranlsationId)]
        [TestCase(nameof(ValidationModelMock.TextValue), "minLength", "TextValue should have at least 2 characters", "")]
        [TestCase(nameof(ValidationModelMock.TextValue), "maxLength", "TextValue should have less then 10 characters", "")]
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
                .HasMinLength(2)
                    .WithMessage(CustomValidationMessage)
                .HasMaxLength(10)
                    .WithMessage(CustomValidationMessage);

            RuleFor(x => x.TextValue)
                .IsRequired()
                    .WithMessage(CustomValidationMessage)
                    .WithMessageTranslationId(CustomValidationMessageTranlsationId)
                .HasMinLength(2)
                .HasMaxLength(10);
        }
    }
}
