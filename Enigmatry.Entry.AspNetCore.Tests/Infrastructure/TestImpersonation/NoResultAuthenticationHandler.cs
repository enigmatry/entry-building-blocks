using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Enigmatry.Entry.AspNetCore.Tests.Infrastructure.TestImpersonation;

/// <summary>
/// Dummy authentication handler, needed to satisfy authentication requirement for authorization (AppAuthorization can not be called without AddAuthentication)
/// Only use this if there is the application doesn't have authentication, but some endpoints have authorization (for example health check endpoint)
/// </summary>
public class NoResultAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "NoResult";

    public NoResultAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());
}
