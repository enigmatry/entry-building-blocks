using System.Reflection;
using Enigmatry.Entry.AspNetCore.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Startup;

public static class ValidationStartupExtensions
{
    public static void AppAddValidation(this IServiceCollection services, Assembly assemblyWithValidators)
    {
        services.AppAddFluentValidation(assemblyWithValidators);
        services.PostConfigure<ApiBehaviorOptions>(options => options.InvalidModelStateResponseFactory = context =>
            context.HttpContext.CreateValidationProblemDetailsResponse(context.ModelState));
    }

    private static IServiceCollection AppAddFluentValidation(this IServiceCollection services,
        Assembly assemblyWithValidators) =>
        services.AddValidatorsFromAssembly(assemblyWithValidators);
}
