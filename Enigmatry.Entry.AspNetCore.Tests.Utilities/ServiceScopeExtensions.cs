﻿using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.AspNetCore.Tests.Utilities;

public static class ServiceScopeExtensions
{
    public static T Resolve<T>(this IServiceScope scope) where T : notnull =>
        scope.ServiceProvider.GetRequiredService<T>();
}
