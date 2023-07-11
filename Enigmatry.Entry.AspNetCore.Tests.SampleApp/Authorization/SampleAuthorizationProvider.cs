using System.Collections.Generic;
using System.Linq;
using Enigmatry.Entry.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;

public class SampleAuthorizationProvider : IAuthorizationProvider<PermissionId>
{
    public bool HasAnyPermission(IEnumerable<PermissionId> permission)
    {
        // Let's assume the current user only has the 'Read' permission or 'Read' enum permission 
        var allowedPermissions = new[] { PermissionId.Read };

        return permission.Any(p => allowedPermissions.Contains(p));
    }
}
