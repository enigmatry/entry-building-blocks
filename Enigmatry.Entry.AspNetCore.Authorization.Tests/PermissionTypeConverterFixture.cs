﻿using System.ComponentModel;
using System.Globalization;
using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using FluentAssertions;
using NUnit.Framework;

namespace Enigmatry.Entry.AspNetCore.Authorization.Tests;

public class PermissionTypeConverterFixture
{
    [Test]
    public void TestIsConversionToPolicyNamePossible()
    {
        Assert.DoesNotThrow(PermissionTypeConverter<PermissionEnum>.EnsureConversionToPolicyNameIsPossible);
        Assert.DoesNotThrow(PermissionTypeConverter<string>.EnsureConversionToPolicyNameIsPossible);
        Assert.DoesNotThrow(PermissionTypeConverter<int>.EnsureConversionToPolicyNameIsPossible);
        Assert.DoesNotThrow(PermissionTypeConverter<CustomPermissionType>.EnsureConversionToPolicyNameIsPossible);
        Assert.Throws<ArgumentException>(PermissionTypeConverter<CustomPermissionTypeWithoutTypeConverter>.EnsureConversionToPolicyNameIsPossible);
    }

    [Test]
    public void TestConvertToPolicyName()
    {
        var policyName = "Read|Write|Delete";

        PermissionTypeConverter<PermissionEnum>
            .ConvertToPolicyName("", new[] { PermissionEnum.Read, PermissionEnum.Write, PermissionEnum.Delete })
            .Should().Be(policyName);

        PermissionTypeConverter<string>
            .ConvertToPolicyName("", new[] { "Read", "Write", "Delete" })
            .Should().Be(policyName);

        PermissionTypeConverter<CustomPermissionType>
            .ConvertToPolicyName("",
                new[]
                {
                    new CustomPermissionType("Read"),
                    new CustomPermissionType("Write"),
                    new CustomPermissionType("Delete")
                })
            .Should().Be(policyName);
    }

    [Test]
    public void TestConvertFromPolicyName()
    {
        var policyName = "Read|Write|Delete";

        PermissionTypeConverter<PermissionEnum>
            .ConvertFromPolicyName("", policyName)
            .Should().Equal(PermissionEnum.Read, PermissionEnum.Write, PermissionEnum.Delete);

        PermissionTypeConverter<string>
            .ConvertFromPolicyName("", policyName)
            .Should().Equal("Read", "Write", "Delete");

        PermissionTypeConverter<CustomPermissionType>
            .ConvertFromPolicyName("", policyName)
            .Should().Equal(new CustomPermissionType("Read"),
                new CustomPermissionType("Write"),
                new CustomPermissionType("Delete"));
    }
}

internal enum PermissionEnum
{
    Read,
    Write,
    Delete
}

[TypeConverter(typeof(CustomPermissionTypeConverter))]
internal record CustomPermissionType(string Name);

internal record CustomPermissionTypeWithoutTypeConverter(string Name);

public class CustomPermissionTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context,
        Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context,
        CultureInfo? culture, object value)
    {
        if (value is string name)
        {
            return new CustomPermissionType(name);
        }
        return null;
    }

    public override object? ConvertTo(ITypeDescriptorContext? context,
        CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is CustomPermissionType permission)
        {
            return permission.Name;
        }
        return null;
    }
}
