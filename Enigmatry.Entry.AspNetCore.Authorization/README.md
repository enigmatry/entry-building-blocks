# Authorization Building Block

Building Block with startup extensions for enabling authorization based on roles and permissions.

## Registration

You can use the `AppAddAuthorization` extension method on `IServiceCollection`.  This will register RequirementHandlers for both Role-based and Permission-based requirements. It will also register a custom `IAuthorizationPolicyProvider` implementation, for mapping Attributes to Requirements. The boolean parameter, can be used to enable or disable authorization entirely (which can be useful for tests):

```cs
 public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AppAddAuthorization(true);
    }
```

## Securing API methods

To secure a method on a controller, you can now use 2 additional Attributes: 
- UserHasRole, which accepts a comma-separated list of strings, representing the roles of which the user must a t least have one.
- UserHasPermission, which accepts a comma-separated list of strings, representing the permissions of which the user must a t least have one.

```cs
    [HttpGet("userNotInRoleIsNotAllowed")]
    [UserHasRole("Admin")]
    public IEnumerable<WeatherForecast> UserNotInRoleIsNotAllowed() => Array.Empty<WeatherForecast>();

    [HttpGet("UserWithPermissionIsAllowed")]
    [UserHasPermission("Read,Write")]
    public IEnumerable<WeatherForecast> UserWithPermissionIsAllowed() => Array.Empty<WeatherForecast>();
```

## Implementing authorization checks

Because the actual check if the current user has the right roles or permissions can be application-specific, this building block only provides an interface `IAuthorizationProvider`, with no default implementation. Application using this building block need to register their own implementation:

```cs
public interface IAuthorizationProvider
{
    public bool HasAnyRole(string[] roles);
    public bool HasAnyPermission(string[] permissions);
}
```

```cs
  services.AddScoped<IAuthorizationProvider, SampleAuthorizationProvider>();
```
