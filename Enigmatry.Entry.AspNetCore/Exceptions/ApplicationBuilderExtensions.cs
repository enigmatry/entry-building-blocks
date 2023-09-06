using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Enigmatry.Entry.AspNetCore.Exceptions;

[PublicAPI]
public static class ApplicationBuilderExtensions
{
    public static void AppUseExceptionHandler(this IApplicationBuilder builder,
        Func<HttpContext, Task<bool>>? onBeforeException = null) =>
        builder.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var handledElsewhere = false;
                if (onBeforeException != null)
                {
                    handledElsewhere = await onBeforeException.Invoke(context);
                }

                if (handledElsewhere)
                {
                    return;
                }

                await ExceptionHandler.HandleExceptionFrom(context);
            });
        });
}
