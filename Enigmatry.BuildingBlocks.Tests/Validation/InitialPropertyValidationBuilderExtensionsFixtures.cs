using Enigmatry.BuildingBlocks.Validation;
using Enigmatry.BuildingBlocks.Validation.PropertyValidations;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Enigmatry.BuildingBlocks.Tests.Validation
{
    [Category("unit")]
    public class InitialPropertyValidationBuilderExtensionsFixtures
    {
        private MockModelValidationConfiguration _validationConfiguration = null!;

        [SetUp]
        public void SetUp() => _validationConfiguration = new MockModelValidationConfiguration();

        [Test]
        public void IsRequired()
        {
            _validationConfiguration
                .RuleFor(x => x.StringField)
                .IsRequired();

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("required");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().BeEmpty();
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.required");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo("required: true");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: is required");
        }

        [Test]
        public void Match()
        {
            _validationConfiguration
                .RuleFor(x => x.StringField)
                .Match(new Regex("/^[A-Z]{4}[1-9]{8}$/mu"));

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("pattern");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().BeEmpty();
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.pattern");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo("pattern: /^[A-Z]{4}[1-9]{8}$/mu");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: is not in valid format");
        }

        [Test]
        public void EmailAddress()
        {
            _validationConfiguration
                .RuleFor(x => x.StringField)
                .EmailAddress();

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("pattern");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().Be("Invalid email address format");
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.pattern.emailAddress");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo(@"pattern: /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("Invalid email address format");
        }

        [Test]
        public void MinLength()
        {
            _validationConfiguration
                .RuleFor(x => x.StringField)
                .MinLength(0);

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("minLength");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().BeEmpty();
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.minLength");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo("minLength: 0");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: should have at least ${field?.templateOptions?.minLength}:min-value: characters");
        }

        [Test]
        public void MaxLength()
        {
            _validationConfiguration
                .RuleFor(x => x.StringField)
                .MaxLength(100);

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("maxLength");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().BeEmpty();
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.maxLength");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo("maxLength: 100");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.maxLength}:max-value: characters");
        }

        [Test]
        public void Length()
        {
            _validationConfiguration
                .RuleFor(x => x.StringField)
                .Length(10);

            _validationConfiguration.ValidationRules.Should().HaveCount(2);

            var minRule = _validationConfiguration.ValidationRules.Single(x => x.FormlyRuleName == "minLength");
            minRule.CustomMessage.Should().BeEmpty();
            minRule.MessageTranslationId.Should().Be("validators.minLength");
            minRule.FormlyTemplateOptions.Should().BeEquivalentTo("minLength: 10");
            minRule.FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: should have at least ${field?.templateOptions?.minLength}:min-value: characters");

            var maxRule = _validationConfiguration.ValidationRules.Single(x => x.FormlyRuleName == "maxLength");
            maxRule.CustomMessage.Should().BeEmpty();
            maxRule.MessageTranslationId.Should().Be("validators.maxLength");
            maxRule.FormlyTemplateOptions.Should().BeEquivalentTo("maxLength: 10");
            maxRule.FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.maxLength}:max-value: characters");
        }

        [Test]
        public void GreaterThen()
        {
            _validationConfiguration
                .RuleFor(x => x.IntField)
                .GreaterThen(0);

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("min");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().BeEmpty();
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.min");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo("type: 'number'", "min: 1");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: value should be more than ${field?.templateOptions?.min}:min-value:");
        }

        [Test]
        public void GreaterOrEqualTo()
        {
            _validationConfiguration
                .RuleFor(x => x.IntField)
                .GreaterOrEqualTo(0);

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("min");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().BeEmpty();
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.min");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo("type: 'number'", "min: 0");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: value should be more than ${field?.templateOptions?.min}:min-value:");
        }

        [Test]
        public void LessThen()
        {
            _validationConfiguration
                .RuleFor(x => x.IntField)
                .LessThen(100);

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("max");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().BeEmpty();
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.max");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo("type: 'number'", "max: 99");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.max}:max-value:");
        }

        [Test]
        public void LessOrEqualTo()
        {
            _validationConfiguration
                .RuleFor(x => x.IntField)
                .LessOrEqualTo(100);

            _validationConfiguration.ValidationRules
                .Should().HaveCount(1);
            _validationConfiguration.ValidationRules.Single().FormlyRuleName
                .Should().Be("max");
            _validationConfiguration.ValidationRules.Single().CustomMessage
                .Should().BeEmpty();
            _validationConfiguration.ValidationRules.Single().MessageTranslationId
                .Should().Be("validators.max");
            _validationConfiguration.ValidationRules.Single().FormlyTemplateOptions
                .Should().BeEquivalentTo("type: 'number'", "max: 100");
            _validationConfiguration.ValidationRules.Single().FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.max}:max-value:");
        }

        [Test]
        public void EqualTo()
        {
            _validationConfiguration
                .RuleFor(x => x.IntField)
                .EqualTo(0);

            _validationConfiguration.ValidationRules
                .Should().HaveCount(2);
            var minRule = _validationConfiguration.ValidationRules.Single(x => x.FormlyRuleName == "min");
            minRule.CustomMessage.Should().BeEmpty();
            minRule.MessageTranslationId.Should().Be("validators.min");
            minRule.FormlyTemplateOptions.Should().BeEquivalentTo("type: 'number'", "min: 0");
            minRule.FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: value should be more than ${field?.templateOptions?.min}:min-value:");

            var maxRule = _validationConfiguration.ValidationRules.Single(x => x.FormlyRuleName == "max");
            maxRule.CustomMessage.Should().BeEmpty();
            maxRule.MessageTranslationId.Should().Be("validators.max");
            maxRule.FormlyTemplateOptions.Should().BeEquivalentTo("type: 'number'", "max: 0");
            maxRule.FormlyValidationMessage
                .Should().Be("${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.max}:max-value:");
        }

        [TestCase("MESSAGE", "")]
        [TestCase("MESSAGE", "MESSAGE_TRANSLATION_ID")]
        public void WithMessage(string message, string messageTranslationId)
        {
            _validationConfiguration
                .RuleFor(x => x.IntField)
                .IsRequired()
                .WithMessage(message, messageTranslationId);

            _validationConfiguration.ValidationRules.Single()
                .CustomMessage.Should().Be(message);
            _validationConfiguration.ValidationRules.Single()
                .MessageTranslationId.Should()
                .Be(String.IsNullOrWhiteSpace(messageTranslationId) ? String.Empty : messageTranslationId);
        }

        [Test]
        public void InvalidGreaterThen()
        {
            Func<IPropertyMessageValidationBuilder<MockModel, DateTime>> lessThenFunc =
                () => (IPropertyMessageValidationBuilder<MockModel, DateTime>)
                    _validationConfiguration.RuleFor(x => x.DateTimeOffsetField).GreaterThen(0);

            lessThenFunc
                .Should()
                .ThrowExactly<InvalidOperationException>()
                .WithMessage($"{nameof(DateTimeOffset)} is not of number format. Only number formats supported.");
        }

        [Test]
        public void InvalidLessThen()
        {
            Func<IPropertyMessageValidationBuilder<MockModel, DateTime>> lessThenFunc =
                () => (IPropertyMessageValidationBuilder<MockModel, DateTime>)
                    _validationConfiguration.RuleFor(x => x.DateTimeOffsetField).LessThen(100);

            lessThenFunc
                .Should()
                .ThrowExactly<InvalidOperationException>()
                .WithMessage($"{nameof(DateTimeOffset)} is not of number format. Only number formats supported.");
        }

        [Test]
        public void InvalidWithMessage()
        {
            Func<IPropertyMessageValidationBuilder<MockModel, DateTime>> lessThenFunc =
                () => (IPropertyMessageValidationBuilder<MockModel, DateTime>)
                    _validationConfiguration.RuleFor(x => x.IntField).LessThen(100).WithMessage("", "test");

            lessThenFunc
                .Should()
                .ThrowExactly<InvalidOperationException>()
                .WithMessage($"{nameof(MockModel.IntField)} validation message cannot be empty.");
        }
    }

    internal class MockModel
    {
        public string StringField { get; set; } = String.Empty;
        public int IntField { get; set; }
        public DateTimeOffset DateTimeOffsetField { get; set; }
    }

    internal class MockModelValidationConfiguration : ValidationConfiguration<MockModel>
    {
        public const string Message = "MESSAGE";
        public const string TranslationId = "TRANSLATION_ID";
    }
}
