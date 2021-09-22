using Enigmatry.BuildingBlocks.Validation;
using FluentAssertions;
using Humanizer;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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
                .Where(x => x.PropertyName == nameof(ValidationModelMock.IntValue).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.IntValue).Camelize())
                .Select(x => x.FormlyRuleName)
                .Should().BeEquivalentTo("required", "min", "max");

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.DoubleValue).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.DoubleValue).Camelize())
                .Select(x => x.FormlyRuleName)
                .Should().BeEquivalentTo("required", "min", "max");

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.StringValue).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.StringValue).Camelize())
                .Select(x => x.FormlyRuleName)
                .Should().BeEquivalentTo("required", "minLength", "maxLength");
        }


        [Test]
        public void ValidationConfigurationForPatterns()
        {
            var validationConfiguration = new PetternsValidationModelMockConfiguration();

            validationConfiguration.ValidationRules
                .Select(x => x.FormlyRuleName)
                .Should().BeEquivalentTo("pattern", "pattern");
            validationConfiguration.ValidationRules
                .Select(x => x.PropertyName.Pascalize())
                .Should().BeEquivalentTo("OtherStringValue", "StringValue");
            validationConfiguration.ValidationRules
                .Select(x => x.CustomMessage)
                .Should().BeEquivalentTo("", "Invalid email address format");
            validationConfiguration.ValidationRules
                .Select(x => x.FormlyValidationMessage)
                .Should().BeEquivalentTo(
                    "${field?.templateOptions?.label}:property-name: is not in valid format",
                    "Invalid email address format"
                );
            validationConfiguration.ValidationRules
                .Select(x => x.FormlyValidationMessage)
                .Should().BeEquivalentTo(
                    "${field?.templateOptions?.label}:property-name: is not in valid format",
                    "Invalid email address format"
                );
            validationConfiguration.ValidationRules
                .Select(x => x.MessageTranslationId)
                .Should().BeEquivalentTo("validators.pattern", "validators.pattern.emailAddress");
        }

        [TestCase(nameof(ValidationModelMock.IntValue), "required", "", "validators.required")]
        [TestCase(nameof(ValidationModelMock.IntValue), "min", ValidationModelMockConfiguration.CustomMessage, "")]
        [TestCase(nameof(ValidationModelMock.IntValue), "max", ValidationModelMockConfiguration.CustomMessage, ValidationModelMockConfiguration.CustomMessageTranlsationId)]
        [TestCase(nameof(ValidationModelMock.StringValue), "required", ValidationModelMockConfiguration.CustomMessage, ValidationModelMockConfiguration.CustomMessageTranlsationId)]
        [TestCase(nameof(ValidationModelMock.StringValue), "minLength", ValidationModelMockConfiguration.CustomMessage, "")]
        [TestCase(nameof(ValidationModelMock.StringValue), "maxLength", "", "validators.maxLength")]
        public void ValidationConfigurationPerValidationRule(
            string propertyName,
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
                .SingleOrDefault(rule => rule.FormlyRuleName == validationRuleName);
            validationRule.Should().NotBeNull();
            validationRule?.CustomMessage.Should().Be(validationMessage);
            validationRule?.MessageTranslationId.Should().Be(validationMessageTranslationId);
        }
    }

    internal class ValidationModelMock
    {
        public int IntValue { get; set; }
        public double DoubleValue { get; set; }
        public DateTimeOffset DateTimeOffsetValue { get; set; }
        public string StringValue { get; set; } = String.Empty;
        public string OtherStringValue { get; set; } = String.Empty;
    }

    internal class ValidationModelMockConfiguration : ValidationConfiguration<ValidationModelMock>
    {
        public const string CustomMessage = "CUSTOM_VALIDATION_MESSAGE";
        public const string CustomMessageTranlsationId = "CUSTOM_VALIDATION_MESSAGE_TRANSLATION_ID";

        public ValidationModelMockConfiguration()
        {
            RuleFor(x => x.IntValue)
                .IsRequired()
                .GreaterThen(0).WithMessage(CustomMessage)
                .LessThen(10).WithMessage(CustomMessage, CustomMessageTranlsationId);

            RuleFor(x => x.DoubleValue)
                .IsRequired()
                .GreaterThen(0).WithMessage(CustomMessage)
                .LessThen(10).WithMessage(CustomMessage, CustomMessageTranlsationId);

            RuleFor(x => x.StringValue)
                .IsRequired().WithMessage(CustomMessage, CustomMessageTranlsationId)
                .MinLength(0).WithMessage(CustomMessage)
                .MaxLength(10);
        }
    }

    internal class PetternsValidationModelMockConfiguration : ValidationConfiguration<ValidationModelMock>
    {
        public PetternsValidationModelMockConfiguration()
        {
            RuleFor(x => x.OtherStringValue).EmailAddress();
            RuleFor(x => x.StringValue).Match(new Regex("/[A-Z]/"));
        }
    }
}
