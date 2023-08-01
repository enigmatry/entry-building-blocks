# Authorization Building Block

Building Block with startup extensions for enabling permission-based authorization

## Registration

You can use the `AppAddAuthorization` extension method on `IServiceCollection`. 
This will register RequirementHandlers for permission-based authorization for the specified permission type (TPermission):


```cs
 public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AppAddAuthorization<TPermission>();
    }
```

### Permission type

Underneath the covers, policy based authentication is used through a custom [`IAuthorizationPolicyProvider`](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider) implementation and permissions are encoded to a policy name.
As a result, the permission type must be capable of converting to and from a string.
Types such as 'String', 'Enum', and 'int' are automatically supported. If you want to use a custom permission type, you must implement [System.ComponentModel.TypeConverter].(https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter).

For the rest of the examples in this document, let's use an enum as the permission type:

```cs
public enum PermissionId
{
    Read,
    Write
}
```

## Securing API methods

To secure a method on a controller, you can now use UserHasPermission with list of requred permissions:

```cs
[HttpGet("UserWithPermissionIsAllowed")]
[UserHasPermission<PermissionId>(PermissionId.Read, PermissionId.Write)]
public IEnumerable<WeatherForecast> UserWithPermissionIsAllowed() => Array.Empty<WeatherForecast>();
```

## Implementing authorization checks

Because the actual check if the current user has the right permissions can be application-specific, this building block only provides an interface `IAuthorizationProvider`, with no default implementation:
```cs
public interface IAuthorizationProvider<in TPermission> where TPermission : notnull
{
    public bool AuthorizePermissions(IEnumerable<TPermission> permissions);
}
```

Applications using this building block need to register their own implementation:

```cs
services.AddScoped<IAuthorizationProvider<PermissionId>, SampleAuthorizationProvider>();
```

```cs
public class SampleAuthorizationProvider : IAuthorizationProvider<PermissionId>
{
    public bool AuthorizePermissions(IEnumerable<PermissionId> permissions) =>
        // Let's assume the current user only has the 'Read' permission
        permissions.Any(p => p == PermissionId.Read);
}
```
