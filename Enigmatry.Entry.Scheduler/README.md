# Scheduler Building Block

Building Block with startup extensions for a scheduler webjob using Quartz jobs.

## Registration

You can use the `AppAddQuartz` extension method on `IServiceCollection` to add a Quartz hosted service to th IoC container. This extension method will also configure Quartz and dynamicall add all the jobs from your scheduler project:

```cs
 public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AppAddQuartz(context.Configuration, Assembly.GetExecutingAssembly());
    }
```
This wil scan your currently executing assembly for all types that match the Quartz jobs that are configured in the appSettings, and provide them to Quartz as a job to be run. 

## Configuration

Example:

```json
  "App": {
    
      "Scheduling": {
        "Host": {
          "quartz.scheduler.instanceName": "Enigmatry.Entry.Scheduler"
        },
        "Jobs": {
          "ExampleScheduledJob": {
            "Cronex": "0 * 0/10 * * ?",
            "RunOnStartup": true
          }
        }
      },
    },
```

For more documentation on configuring Quartz and Cron expressions: https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#quartz-net-configuration-reference
