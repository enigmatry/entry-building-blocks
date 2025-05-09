﻿using System.ComponentModel.DataAnnotations;
using Enigmatry.Entry.Core.Helpers;
using NUnit.Framework;
using Shouldly;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Enigmatry.Entry.Core.Tests;

[Category("unit")]
public class EnumExtensionsFixture
{
    [TestCase(TestType.None, "")]
    [TestCase(TestType.EnumWithDisplayName, "Enum with display name")]
    [TestCase(TestType.EnumWithDisplayNameAndDescription, "Enum with display name and description")]
    public void TestGetDisplayName(TestType value, string expectedDisplayName) =>
        value.GetDisplayName().ShouldBe(expectedDisplayName);

    [TestCase(TestType.None, "")]
    [TestCase(TestType.EnumWithDescription, "Enum with description")]
    [TestCase(TestType.EnumWithDisplayNameAndDescription, "Enum with display name and description2")]
    public void TestGetDescription(TestType value, string expectedDescription) =>
        value.GetDescription().ShouldBe(expectedDescription);

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
