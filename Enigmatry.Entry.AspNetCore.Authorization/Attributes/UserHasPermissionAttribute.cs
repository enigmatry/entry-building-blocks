using Microsoft.AspNetCore.Authorization;
#pragma warning disable CA1813 // Avoid unsealed attributes

namespace Enigmatry.Entry.AspNetCore.Authorization.Attributes;

/// <summary>
/// Authorization attribute that specifies a list of permissions that are required for the endpoint.
/// Checking of the permissions, whether "all" or "any of" are required, depends on your implementation of the <see cref="IAuthorizationProvider{T}"/> interface. 
/// </summary>
public class UserHasPermissionAttribute<TPermission> : AuthorizeAttribute where TPermission : notnull
{
    public const string PolicyPrefix = nameof(UserHasPermissionAttribute<TPermission>);

    /// <summary>
    /// Initializes a new instance of the <see cref="UserHasPermissionAttribute{T}"/> class.
    /// </summary>
    /// <param name="permissions">List of permissions that are required to access the resource.</param>
    /// 
    public UserHasPermissionAttribute(params TPermission[] permissions)
        : base(PermissionTypeConverter<TPermission>.ConvertToPolicyName(PolicyPrefix, permissions))
    {
        if (permissions.Length == 0)
        {
            throw new ArgumentException("Permissions cannot be an empty collection.", nameof(permissions));
        }
    }
}
