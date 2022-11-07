# Health Checks Building Block

Building Block for using the ASP.Net Health Checks 

## Registration

You can use the `AppAddHealthChecks` extension method on `IServiceCollection` to add a `HealthChecksService` to th IoC container. This returns a `HealthChecksBuilder`, which can be used to configure the health checks  :

```cs
 public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AppAddHealthChecks(configuration)
            .AddDbContextCheck<YessaBaseContext>()
            .AddDiskStorageHealthCheck
                // etc
    }
```
Next, you also need to call the `AppMapHealthCheck` method to register the health checks endpoint, providing an instance of the configuration object.

```cs
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.AppMapHealthCheck(_configuration);
        });
```

## Configuration

Example:

```json
  "App": {
    
      "HealthChecks": {
        "MaximumAllowedMemoryInMB": "200",
        "TokenAuthorizationEnabled": true,
        "RequiredToken": "__healthChecksToken__"
      }
    },
```

- The token (if enabled) needs to be a random string, like a GUID. This then needs to be sent as a query parameter with any request, to be authorized: `healthcheckurl?token=[token]`.
This can be used to setup Application Insights availability monitoring.
