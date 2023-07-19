using NUnit.Framework;
using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using FluentAssertions;

namespace Enigmatry.Entry.AspNetCore.Authorization.Tests;

public class PolicyNamesConverterFixture
{
    [Test]
    public void TestCanConvert()
    {
        PolicyNameConverter<PermissionEnum>.CanConvertToPolicyName().Should().BeTrue();
        PolicyNameConverter<string>.CanConvertToPolicyName().Should().BeTrue();
    }

    [Test]
    public void TestConvertToPolicyName()
    {
        PolicyNameConverter<PermissionEnum>
            .ConvertToPolicyName("", new[] { PermissionEnum.Read, PermissionEnum.Write, PermissionEnum.Delete })
            .Should().Be("Read,Write,Delete");

        PolicyNameConverter<string>
            .ConvertToPolicyName("", new[] { "Read", "Write", "Delete" })
            .Should().Be("Read,Write,Delete");
    }

    [Test]
    public void TestConvertFromPolicyName()
    {
        PolicyNameConverter<PermissionEnum>
            .ConvertFromPolicyName("", "Read,Write,Delete")
            .Should().Equal(PermissionEnum.Read, PermissionEnum.Write, PermissionEnum.Delete);

        PolicyNameConverter<string>
            .ConvertFromPolicyName("", "Read,Write,Delete")
            .Should().Equal("Read", "Write", "Delete");
    }
}

internal enum PermissionEnum
{
    Read,
    Write,
    Delete
}
