using System.Collections.Generic;
using System.Linq;
using Enigmatry.Entry.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;

public class SampleAuthorizationProvider : IAuthorizationProvider<PermissionId>
{
    public bool AuthorizePermissions(IEnumerable<PermissionId> permissions) =>
        permissions.Any(p => p == PermissionId.Read); // Let's assume the current user only has the 'Read' permission
}
