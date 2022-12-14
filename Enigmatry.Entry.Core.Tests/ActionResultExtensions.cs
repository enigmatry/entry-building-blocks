using Microsoft.AspNetCore.Mvc;

namespace Enigmatry.Entry.Core.Tests;
internal static class ActionResultExtensions
{
    internal static IEnumerable<string> GetErrorMessages(this IActionResult actionResult)
    {
        var resultValue = (actionResult as BadRequestObjectResult)?.Value;

        return resultValue is ValidationProblemDetails details ? ErrorsFrom(details) : Enumerable.Empty<string>();
    }

    private static IEnumerable<string> ErrorsFrom(ValidationProblemDetails details) => details.Errors.Values.SelectMany(v => v);
}
