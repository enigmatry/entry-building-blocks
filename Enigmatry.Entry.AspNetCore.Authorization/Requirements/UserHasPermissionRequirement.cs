﻿using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Authorization.Requirements;

internal class UserHasPermissionRequirement : IAuthorizationRequirement
{
    public UserHasPermissionRequirement(string permissions)
    {
        Permissions = permissions;
    }

    public string Permissions { get; }
}
