using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Enigmatry.Entry.GraphApi.Injection;

internal class FixedTokenProvider(string token) : IAccessTokenProvider
{
    private readonly string _token = token ?? throw new ArgumentNullException(nameof(token));

    public Task<string> GetAuthorizationTokenAsync(Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = new()) => Task.FromResult(_token);

    public AllowedHostsValidator AllowedHostsValidator { get; } = new();
}
