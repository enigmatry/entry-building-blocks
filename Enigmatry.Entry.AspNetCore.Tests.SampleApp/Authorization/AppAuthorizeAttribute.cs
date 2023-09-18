using Enigmatry.Entry.AspNetCore.Authorization.Attributes;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;

public sealed class AppAuthorizeAttribute : UserHasPermissionAttribute<PermissionId>
{
    public AppAuthorizeAttribute(params PermissionId[] permissions) : base(permissions)
    {
    }
}
