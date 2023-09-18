using System.Security.Claims;

namespace Enigmatry.Entry.AspNetCore.Tests.Infrastructure.TestImpersonation;

public static class TestUserData
{
    public static readonly Guid TestUserId = new("e984fba2-3b00-4c0b-9612-9b61a7d12967");
    private const string TestUserName = "test_user@xyz.com";

    public static ClaimsPrincipal CreateClaimsPrincipal() =>
        new(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, value: TestUserName)
        }, "TestAuth"));
}
