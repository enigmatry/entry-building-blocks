using Enigmatry.BuildingBlocks.Validation;
using FluentAssertions;
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

            var validationRules = validationConfiguration.GetValidationRules();

            validationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.NumberValue))
                .Should().HaveCount(3);
            validationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.NumberValue))
                .Select(x => x.Name)
                .Should().BeEquivalentTo("required", "min", "max");

            validationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.TextValue))
                .Should().HaveCount(3);
            validationRules
                .Where(x => x.PropertyName == nameof(ValidationModelMock.TextValue))
                .Select(x => x.Name)
                .Should().BeEquivalentTo("required", "minLength", "maxLength");

            validationRules
                .All(x => !String.IsNullOrWhiteSpace(x.Message))
                .Should().BeTrue();
        }
    }

    public class ValidationModelMock
    {
        public int NumberValue { get; set; }
        public string TextValue { get; set; } = String.Empty;
    }

    public class ValidationModelMockConfiguration : ValidationConfiguration<ValidationModelMock>
    {
        public ValidationModelMockConfiguration()
        {
            RuleFor(x => x.NumberValue)
                .IsRequired("Required field")
                .HasMinLength(2, "Min length 1")
                .HasMaxLength(10, "Min length 10");
            RuleFor(x => x.TextValue)
                .IsRequired()
                .HasMinLength(2)
                .HasMaxLength(10);
        }
    }
}
