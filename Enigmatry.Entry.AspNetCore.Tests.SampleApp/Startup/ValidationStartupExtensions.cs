using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Startup;

public static class ValidationStartupExtensions
{
    public static IServiceCollection AppAddFluentValidation(this IServiceCollection services,
        Assembly assemblyWithValidators) =>
        services.AddValidatorsFromAssembly(assemblyWithValidators);
}
