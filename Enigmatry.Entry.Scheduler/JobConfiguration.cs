using Microsoft.Extensions.Configuration;
using System.Configuration;
using Enigmatry.Entry.Core.Helpers;

namespace Enigmatry.Entry.Scheduler;

internal class JobConfiguration
{
    private readonly IConfigurationSection _jobConfiguration;

    public JobConfiguration(string jobName, Type jobType, IConfigurationSection jobConfiguration)
    {
        _jobConfiguration = jobConfiguration;
        JobName = jobName;
        JobType = jobType;

        EnsureConfigurationIsValid();
    }

    private void EnsureConfigurationIsValid()
    {
        if (!_jobConfiguration.Exists())
        {
            throw new ConfigurationErrorsException($"Configuration Section '{SectionName}' is not found");
        }

        if (!Cronex.HasContent())
        {
            throw new ConfigurationErrorsException($"Missing 'Cronex' value in configuration for configuration section: '{SectionName}'");
        }
    }

    public string JobName { get; }
    private string SectionName => JobName;
    public Type JobType { get; }

    public bool RunOnStartup => _jobConfiguration.GetValue<bool?>("RunOnStartup") ?? false;
    public bool JobEnabled => _jobConfiguration.GetValue<bool?>("Enabled") ?? true;
    public string Cronex => _jobConfiguration["Cronex"];

    internal T GetSchedulingJobArgumentsValue<T>() where T : new()
    {
        var section = _jobConfiguration.GetSection("Args");
        return section == null ? new T() : section.Get<T>();
    }

    internal static string GetJobName(Type jobType) => jobType.Name;
}
