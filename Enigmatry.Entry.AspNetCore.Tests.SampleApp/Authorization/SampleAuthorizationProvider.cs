using System.Collections.Generic;
using System.Linq;
using Enigmatry.Entry.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;

public class SampleAuthorizationProvider : IAuthorizationProvider<PermissionId>
{
    public bool AuthorizePermissions(IEnumerable<PermissionId> permissions) =>
        // Let's assume the current user only has the 'Read' permission
        permissions.Any(p => p == PermissionId.Read);
}
