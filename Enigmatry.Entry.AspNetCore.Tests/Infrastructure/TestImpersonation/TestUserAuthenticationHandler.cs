using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.AspNetCore.Tests.Infrastructure.TestImpersonation;

public class TestUserAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
{
    public const string AuthenticationScheme = "TestUserAuth";

    public TestUserAuthenticationHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var testPrincipal = Options.TestPrincipalFactory();

        var authResult = testPrincipal != null
            ? AuthenticatedUserResult(testPrincipal)
            : AuthenticateResult.NoResult();  // no user authenticated

        return Task.FromResult(authResult);
    }

    private static AuthenticateResult AuthenticatedUserResult(ClaimsPrincipal testPrincipal)
        => AuthenticateResult.Success(new AuthenticationTicket(testPrincipal, AuthenticationScheme));
}

public class TestAuthenticationOptions : AuthenticationSchemeOptions
{
    public Func<ClaimsPrincipal?> TestPrincipalFactory { get; set; } = () => null;
}
