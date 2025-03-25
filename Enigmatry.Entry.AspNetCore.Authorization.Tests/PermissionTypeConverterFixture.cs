using System.ComponentModel;
using System.Globalization;
using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using NUnit.Framework;
using Shouldly;

namespace Enigmatry.Entry.AspNetCore.Authorization.Tests;

[NUnit.Framework.Category("unit")]
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
            .ShouldBe(policyName);

        PermissionTypeConverter<string>
            .ConvertToPolicyName("", new[] { "Read", "Write", "Delete" })
            .ShouldBe(policyName);

        PermissionTypeConverter<CustomPermissionType>
            .ConvertToPolicyName("",
                new[]
                {
                    new CustomPermissionType("Read"),
                    new CustomPermissionType("Write"),
                    new CustomPermissionType("Delete")
                })
            .ShouldBe(policyName);
    }

    [Test]
    public void TestConvertFromPolicyName()
    {
        var policyName = "Read|Write|Delete";

        PermissionTypeConverter<PermissionEnum>
            .ConvertFromPolicyName("", policyName)
            .ShouldBe([PermissionEnum.Read, PermissionEnum.Write, PermissionEnum.Delete]);

        PermissionTypeConverter<string>
            .ConvertFromPolicyName("", policyName)
            .ShouldBe(["Read", "Write", "Delete"]);

        PermissionTypeConverter<CustomPermissionType>
            .ConvertFromPolicyName("", policyName)
            .ShouldBe([new CustomPermissionType("Read"),
                new CustomPermissionType("Write"),
                new CustomPermissionType("Delete")]);
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
