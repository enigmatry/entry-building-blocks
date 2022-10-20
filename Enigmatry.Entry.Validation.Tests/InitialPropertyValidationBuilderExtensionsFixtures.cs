using System.Globalization;
using System.Text.RegularExpressions;
using Enigmatry.Entry.Validation.PropertyValidations;
using Enigmatry.Entry.Validation.ValidationRules;
using FluentAssertions;
using Humanizer;
using NUnit.Framework;

namespace Enigmatry.Entry.Validation.Tests;

[Category("unit")]
public class InitialPropertyValidationBuilderExtensionsFixtures
{
    private const int MinIntField = 1;
    private const int MaxIntField = 100;
    private const double MinDoubleField = 1.5;
    private const double MaxDoubleField = 100.5;
    private const byte MinByteField = 10;
    private const byte MaxByteField = 100;
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
            .GreaterThen(MinIntField);
        _validationConfiguration
            .RuleFor(x => x.DoubleField)
            .GreaterThen(MinDoubleField);
        _validationConfiguration
            .RuleFor(x => x.ByteField)
            .GreaterThen(MinByteField);

        _validationConfiguration.ValidationRules.Should().HaveCount(3);

        AssertNumbercMinValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.IntField)), MinIntField, false);
        AssertNumbercMinValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.DoubleField)), MinDoubleField, false);
        AssertNumbercMinValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.ByteField)), MinByteField, false);
    }

    [Test]
    public void GreaterOrEqualTo()
    {
        _validationConfiguration
            .RuleFor(x => x.IntField)
            .GreaterOrEqualTo(MinIntField);
        _validationConfiguration
            .RuleFor(x => x.DoubleField)
            .GreaterOrEqualTo(MinDoubleField);
        _validationConfiguration
            .RuleFor(x => x.ByteField)
            .GreaterOrEqualTo(MinByteField);

        _validationConfiguration.ValidationRules.Should().HaveCount(3);

        AssertNumbercMinValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.IntField)), MinIntField, true);
        AssertNumbercMinValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.DoubleField)), MinDoubleField, true);
        AssertNumbercMinValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.ByteField)), MinByteField, true);
    }

    [Test]
    public void LessThen()
    {
        _validationConfiguration
            .RuleFor(x => x.IntField)
            .LessThen(MaxIntField);
        _validationConfiguration
            .RuleFor(x => x.DoubleField)
            .LessThen(MaxDoubleField);
        _validationConfiguration
            .RuleFor(x => x.ByteField)
            .LessThen(MaxByteField);

        _validationConfiguration.ValidationRules.Should().HaveCount(3);

        AssertNumbercMaxValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.IntField)), MaxIntField, false);
        AssertNumbercMaxValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.DoubleField)), MaxDoubleField, false);
        AssertNumbercMaxValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.ByteField)), MaxByteField, false);
    }

    [Test]
    public void LessOrEqualTo()
    {
        _validationConfiguration
            .RuleFor(x => x.IntField)
            .LessOrEqualTo(MaxIntField);
        _validationConfiguration
            .RuleFor(x => x.DoubleField)
            .LessOrEqualTo(MaxDoubleField);
        _validationConfiguration
            .RuleFor(x => x.ByteField)
            .LessOrEqualTo(MaxByteField);

        _validationConfiguration.ValidationRules.Should().HaveCount(3);

        AssertNumbercMaxValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.IntField)), MaxIntField, true);
        AssertNumbercMaxValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.DoubleField)), MaxDoubleField, true);
        AssertNumbercMaxValidationRule(GetRuleByPropertyName(nameof(ValidationMockModel.ByteField)), MaxByteField, true);
    }

    [Test]
    public void EqualToInt()
    {
        _validationConfiguration
            .RuleFor(x => x.IntField)
            .EqualTo(MinIntField);

        _validationConfiguration.ValidationRules.Should().HaveCount(2);

        AssertNumbercMinValidationRule(GetRuleByFormlyRuleName("min"), MinIntField, true);
        AssertNumbercMaxValidationRule(GetRuleByFormlyRuleName("max"), MinIntField, true);
    }

    [Test]
    public void EqualToDouble()
    {
        _validationConfiguration
            .RuleFor(x => x.DoubleField)
            .EqualTo(MinDoubleField);

        _validationConfiguration.ValidationRules.Should().HaveCount(2);

        AssertNumbercMinValidationRule(GetRuleByFormlyRuleName("min"), MinDoubleField, true);
        AssertNumbercMaxValidationRule(GetRuleByFormlyRuleName("max"), MinDoubleField, true);
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
    public void InvalidWithMessage()
    {
        Func<IPropertyValidationBuilder<ValidationMockModel, DateTimeOffset>> lessThenFunc =
            () => (IPropertyValidationBuilder<ValidationMockModel, DateTimeOffset>)
                _validationConfiguration.RuleFor(x => x.IntField).LessThen(100).WithMessage("", "test");

        lessThenFunc
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage($"{nameof(ValidationMockModel.IntField)} validation message cannot be empty.");
    }

    private static void AssertNumbercMinValidationRule<T>(IFormlyValidationRule rule, T value, bool isEqual)
    {
        rule.FormlyRuleName
            .Should().Be("min");
        rule.CustomMessage
            .Should().BeEmpty();
        rule.MessageTranslationId
            .Should().Be("validators.min");
        rule.FormlyTemplateOptions
            .Should().BeEquivalentTo("type: 'number'", $"min: {String.Format(CultureInfo.InvariantCulture, "{0}", value)}{(isEqual ? "" : $" + {GetIncrement<T>()}")}");
        rule.FormlyValidationMessage
            .Should().Be("${field?.templateOptions?.label}:property-name: value should be more than ${field?.templateOptions?.min}:min-value:");
    }

    private static void AssertNumbercMaxValidationRule<T>(IFormlyValidationRule rule, T value, bool isEqual)
    {
        rule.FormlyRuleName
            .Should().Be("max");
        rule.CustomMessage
            .Should().BeEmpty();
        rule.MessageTranslationId
            .Should().Be("validators.max");
        rule.FormlyTemplateOptions
            .Should().BeEquivalentTo("type: 'number'", $"max: {String.Format(CultureInfo.InvariantCulture, "{0}", value)}{(isEqual ? "" : $" - {GetIncrement<T>()}")}");
        rule.FormlyValidationMessage
            .Should().Be("${field?.templateOptions?.label}:property-name: value should be less than ${field?.templateOptions?.max}:max-value:");
    }

    private IFormlyValidationRule GetRuleByPropertyName(string propertyName) =>
        _validationConfiguration.ValidationRules.Single(x => x.PropertyName == propertyName.Camelize());

    private IFormlyValidationRule GetRuleByFormlyRuleName(string formlyRuleName) =>
        _validationConfiguration.ValidationRules.Single(x => x.FormlyRuleName == formlyRuleName);

    private static string GetIncrement<T>() =>
        new[] { typeof(float), typeof(decimal), typeof(double) }.Contains(typeof(T)) ? "0.1" : "1";
}

internal class MockModelValidationConfiguration : ValidationConfiguration<ValidationMockModel> { }
