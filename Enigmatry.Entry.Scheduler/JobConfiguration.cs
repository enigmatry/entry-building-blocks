using Microsoft.Extensions.Configuration;
using System.Configuration;
using Enigmatry.Entry.Core.Helpers;

namespace Enigmatry.Entry.Scheduler;

internal class JobConfiguration
{
    private readonly IConfigurationSection _section;

    public JobConfiguration(string jobName, Type jobType, JobSettings settings, IConfigurationSection section)
    {
        JobName = jobName;
        JobType = jobType;
        Settings = settings;
        _section = section;

        EnsureConfigurationIsValid();
    }

    private void EnsureConfigurationIsValid()
    {
        if (!_section.Exists())
        {
            throw new ConfigurationErrorsException($"Configuration Section '{JobName}' is not found");
        }

        if (!Settings.Cronex.HasContent())
        {
            throw new ConfigurationErrorsException(
                $"Missing 'Cronex' value in configuration for configuration section: '{JobName}'");
        }
    }

    public string JobName { get; }
    public Type JobType { get; }
    public JobSettings Settings { get; }

    internal T GetSchedulingJobArgumentsValue<T>() where T : new()
    {
        var section = _section.GetSection("Args");
        var value = section.Get<T>();
        return value == null ? new T() : value;
    }

    internal static string GetJobName(Type jobType) => jobType.Name;
}
