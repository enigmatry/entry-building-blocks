﻿using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Enigmatry.Entry.AspNetCore.Security;

[PublicAPI]
public static class ApplicationBuilderExtensions
{
    [Obsolete("Use UseEntryHttps instead")]
    public static void AppUseHttps(this IApplicationBuilder builder, IHostEnvironment environment) =>
        builder.UseEntryHttps(environment);


    public static void UseEntryHttps(this IApplicationBuilder builder, IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            builder.UseHsts();
        }

        builder.UseHttpsRedirection();
    }
}
