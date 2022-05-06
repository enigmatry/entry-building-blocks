using System.ComponentModel.DataAnnotations;
using Enigmatry.BuildingBlocks.Core.Helpers;
using FluentAssertions;
using NUnit.Framework;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Enigmatry.BuildingBlocks.Tests.Core
{
    [Category("unit")]
    public class EnumExtensionsFixture
    {
        [TestCase(TestType.None, "")]
        [TestCase(TestType.EnumWithDisplayName, "Enum with display name")]
        [TestCase(TestType.EnumWithDisplayNameAndDescription, "Enum with display name and description")]
        public void TestGetDisplayName(TestType value, string expectedDisplayName) =>
            value.GetDisplayName().Should().Be(expectedDisplayName);

        [TestCase(TestType.None, "")]
        [TestCase(TestType.EnumWithDescription, "Enum with description")]
        [TestCase(TestType.EnumWithDisplayNameAndDescription, "Enum with display name and description2")]
        public void TestGetDescription(TestType value, string expectedDescription) =>
            value.GetDescription().Should().Be(expectedDescription);

        public enum TestType
        {
            None = 0,

            [Display(Name = "Enum with display name")]
            EnumWithDisplayName = 1,

            [Description("Enum with description")]
            EnumWithDescription = 2,

            [Display(Name = "Enum with display name and description")]
            [Description("Enum with display name and description2")]
            EnumWithDisplayNameAndDescription = 5,
        }
    }
}
