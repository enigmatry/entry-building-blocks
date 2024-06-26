﻿using Enigmatry.Entry.TemplatingEngine.Liquid.Filters;
using Enigmatry.Entry.TemplatingEngine.Liquid.ValueConverters;
using Fluid;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.TemplatingEngine.Liquid;

public static class TemplatingEngineStartupExtensions
{
    public static void AddLiquidTemplatingEngine(this IServiceCollection services, Action<FluidTemplateEngineOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddSingleton<FluidParser>();
        services.AddSingleton<IFluidTemplateProvider, FluidTemplateProvider>();
        services.AddScoped<ITemplatingEngine, FluidTemplatingEngine>();

        services.AddScoped<IFluidValueConverter, EnumValueConverter>();
        services.AddScoped<IFluidValueConverter, DateTimeValueConverter>();
        services.AddScoped<IFluidValueConverter, DateTimeOffsetValueConverter>();

        services.AddScoped<IFluidFilter, FormatNumberFilter>();
    }
}
