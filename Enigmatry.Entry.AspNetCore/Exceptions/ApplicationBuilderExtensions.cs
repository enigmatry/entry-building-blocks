using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace Enigmatry.Entry.AspNetCore.Exceptions;

[PublicAPI]
public static class ApplicationBuilderExtensions
{
    public static void AppUseExceptionHandler(this IApplicationBuilder builder, Func<HttpContext, bool>? onBeforeException = null) =>
        builder.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var handledElsewhere = onBeforeException?.Invoke(context) ?? false;
                if (handledElsewhere)
                {
                    return;
                }

                await ExceptionHandler.HandleExceptionFrom(context);
            });
        });
}
