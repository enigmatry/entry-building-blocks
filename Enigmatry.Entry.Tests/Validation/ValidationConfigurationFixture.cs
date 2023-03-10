using Enigmatry.Entry.Validation;
using FluentAssertions;
using Humanizer;
using NUnit.Framework;
using System.Linq;
using System.Text.RegularExpressions;

namespace Enigmatry.Entry.Tests.Validation
{
    [Category("unit")]
    public class ValidationConfigurationFixture
    {
        [Test]
        public void ValidationConfiguration()
        {
            var validationConfiguration = new MockValidationModelConfiguration();

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationMockModel.IntField).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationMockModel.IntField).Camelize())
                .Select(x => x.FormlyRuleName)
                .Should().BeEquivalentTo("required", "min", "max");

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationMockModel.DoubleField).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationMockModel.DoubleField).Camelize())
                .Select(x => x.FormlyRuleName)
                .Should().BeEquivalentTo("required", "min", "max");

            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationMockModel.StringField).Camelize())
                .Should().HaveCount(3);
            validationConfiguration.ValidationRules
                .Where(x => x.PropertyName == nameof(ValidationMockModel.StringField).Camelize())
                .Select(x => x.FormlyRuleName)
                .Should().BeEquivalentTo("required", "minLength", "maxLength");
        }

        [TestCase(nameof(ValidationMockModel.IntField), "required", "", "validators.required")]
        [TestCase(nameof(ValidationMockModel.IntField), "min", MockValidationModelConfiguration.CustomMessage, "")]
        [TestCase(nameof(ValidationMockModel.IntField), "max", MockValidationModelConfiguration.CustomMessage, MockValidationModelConfiguration.CustomMessageTranlsationId)]
        [TestCase(nameof(ValidationMockModel.StringField), "required", MockValidationModelConfiguration.CustomMessage, MockValidationModelConfiguration.CustomMessageTranlsationId)]
        [TestCase(nameof(ValidationMockModel.StringField), "minLength", MockValidationModelConfiguration.CustomMessage, "")]
        [TestCase(nameof(ValidationMockModel.StringField), "maxLength", "", "validators.maxLength")]
        public void ValidationConfigurationPerValidationRule(
            string propertyName,
            string validationRuleName,
            string validationMessage,
            string validationMessageTranslationId)
        {
            var validationConfiguration = new MockValidationModelConfiguration();

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

        [Test]
        public void ValidationConfigurationForPatterns()
        {
            var validationConfiguration = new MockValidationModelWithPatternsConfiguration();

            validationConfiguration.ValidationRules
                .Select(x => x.PropertyName.Pascalize())
                .Should().BeEquivalentTo(nameof(ValidationMockModel.StringField), nameof(ValidationMockModel.OtherStringField));
            validationConfiguration.ValidationRules
                .Select(x => x.FormlyRuleName)
                .Should().BeEquivalentTo("pattern", "pattern");
            validationConfiguration.ValidationRules
                .Where(x => x.HasCustomMessage)
                .Select(x => $"{x.PropertyName.Pascalize()}: {x.CustomMessage}")
                .Should().BeEquivalentTo($"{nameof(ValidationMockModel.OtherStringField)}: Invalid email address format");
            validationConfiguration.ValidationRules
                .Where(x => x.HasMessageTranslationId)
                .Select(x => $"{x.PropertyName.Pascalize()}: {x.MessageTranslationId}")
                .Should().BeEquivalentTo(
                    $"{nameof(ValidationMockModel.StringField)}: validators.pattern",
                    $"{nameof(ValidationMockModel.OtherStringField)}: validators.pattern.emailAddress"
                );
            validationConfiguration.ValidationRules
                .Select(x => x.FormlyValidationMessage)
                .Should().BeEquivalentTo(
                    "${field?.templateOptions?.label}:property-name: is not in valid format",
                    "Invalid email address format"
                );
        }

        [Test]
        public void ValidationConfigurationForNullables()
        {
            var validationConfiguration = new MockValidationModelWithNullablesConfiguration();

            validationConfiguration.ValidationRules
                .Select(x => x.PropertyName.Pascalize()).Distinct()
                .Should().BeEquivalentTo(
                    nameof(ValidationMockModel.NullableIntField),
                    nameof(ValidationMockModel.NullableDoubleField),
                    nameof(ValidationMockModel.NullableByteField)
                );
            validationConfiguration.ValidationRules
                .Select(x => x.FormlyRuleName).Distinct()
                .Should().BeEquivalentTo("required", "min", "max");
            validationConfiguration.ValidationRules
                .All(x => x.HasMessageTranslationId).Should().BeTrue();
            validationConfiguration.ValidationRules
                .All(x => x.HasCustomMessage).Should().BeFalse();
            validationConfiguration.ValidationRules
                .Select(x => x.FormlyValidationMessage).Distinct()
                .Should().BeEquivalentTo(
                    "${field?.templateOptions?.label}:property-name: is required",
                    "${field?.templateOptions?.label}:property-name: value should be more than ${field?.templateOptions?.min}:min-value:",
                    "${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.max}:max-value:"
                );
        }
    }

    #region Mocks

    internal class MockValidationModelConfiguration : ValidationConfiguration<ValidationMockModel>
    {
        public const string CustomMessage = "CUSTOM_VALIDATION_MESSAGE";
        public const string CustomMessageTranlsationId = "CUSTOM_VALIDATION_MESSAGE_TRANSLATION_ID";

        public MockValidationModelConfiguration()
        {
            RuleFor(x => x.IntField)
                .IsRequired()
                .GreaterThen(0).WithMessage(CustomMessage)
                .LessThen(10).WithMessage(CustomMessage, CustomMessageTranlsationId);

            RuleFor(x => x.DoubleField)
                .IsRequired()
                .GreaterThen(0.5).WithMessage(CustomMessage)
                .LessThen(10).WithMessage(CustomMessage, CustomMessageTranlsationId);

            RuleFor(x => x.StringField)
                .IsRequired().WithMessage(CustomMessage, CustomMessageTranlsationId)
                .MinLength(0).WithMessage(CustomMessage)
                .MaxLength(10);
        }
    }

    internal class MockValidationModelWithPatternsConfiguration : ValidationConfiguration<ValidationMockModel>
    {
        public MockValidationModelWithPatternsConfiguration()
        {
            RuleFor(x => x.OtherStringField).EmailAddress();
            RuleFor(x => x.StringField).Match(new Regex("/[A-Z]/"));

        }
    }

    internal class MockValidationModelWithNullablesConfiguration : ValidationConfiguration<ValidationMockModel>
    {
        public MockValidationModelWithNullablesConfiguration()
        {
            RuleFor(x => x.NullableIntField).IsRequired().GreaterThen(1).LessThen(10);
            RuleFor(x => x.NullableDoubleField).IsRequired().GreaterThen(1.1).LessThen(10.1);
            RuleFor(x => x.NullableByteField).IsRequired().GreaterThen((byte)1).LessThen((byte)10);

        }
    }

    #endregion Mocks
}
