using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Blueprint.BuildingBlocks.AspNetCore.Filters
{
    public static class HttpContextExtensions
    {
        public static T Resolve<T>(this HttpContext httpContext) where T : notnull =>
            httpContext.RequestServices.GetRequiredService<T>();
    }
}
