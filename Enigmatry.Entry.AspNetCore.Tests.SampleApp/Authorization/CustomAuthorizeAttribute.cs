using System.Linq;
using Enigmatry.Entry.AspNetCore.Authorization.Attributes;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;

public sealed class CustomAuthorizeAttribute : UserHasPermissionAttribute
{
    public CustomAuthorizeAttribute(params CustomPermissionId[] permissionIds)
        : base(permissionIds.Select(p => p.ToString()).ToArray())
    {
    }
}
