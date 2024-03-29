﻿using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.HealthChecks.Authorization;

internal class TokenRequirement : IAuthorizationRequirement
{
    public const string Name = "HealthCheckToken";

    public TokenRequirement(string token)
    {
        Token = token;
    }

    public string Token { get; }
}
