using System;
using System.Linq;
using Enigmatry.Entry.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp;

public class SampleAuthorizationProvider : IAuthorizationProvider
{
    public bool HasAnyRole(string[] roles) =>
        // Let's assume the current user only has the role of 'Tester'
        roles.Contains("Tester", StringComparer.Ordinal);

    public bool HasAnyPermission(string[] permission) =>
        // Let's assume the current user only has the 'Read' permission
        permission.Contains("Read", StringComparer.Ordinal);
}
