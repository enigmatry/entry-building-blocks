using NUnit.Framework;
using Enigmatry.Entry.AspNetCore.Authorization.Attributes;
using FluentAssertions;

namespace Enigmatry.Entry.AspNetCore.Authorization.Tests;

public class PolicyNamesConverterFixture
{


    [Test]
    public void TestConvertToPolicyName()
    {
        PermissionsConverter<PermissionEnum>
            .FormatToPolicyName("", new[] { PermissionEnum.Read, PermissionEnum.Write, PermissionEnum.Delete })
            .Should().Be("Read,Write,Delete");

        PermissionsConverter<string>
            .FormatToPolicyName("", new[] { "Read", "Write", "Delete" })
            .Should().Be("Read,Write,Delete");
    }

    [Test]
    public void TestConvertFromPolicyName()
    {
        PermissionsConverter<PermissionEnum>
            .ParseFromPolicyName("", "Read,Write,Delete")
            .Should().Equal(PermissionEnum.Read, PermissionEnum.Write, PermissionEnum.Delete);

        PermissionsConverter<string>
            .ParseFromPolicyName("", "Read,Write,Delete")
            .Should().Equal("Read", "Write", "Delete");
    }
}

internal enum PermissionEnum
{
    Read,
    Write,
    Delete
}
