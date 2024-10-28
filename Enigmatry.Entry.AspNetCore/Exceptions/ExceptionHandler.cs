using Enigmatry.Entry.AspNetCore.Filters;
using Enigmatry.Entry.AspNetCore.Validation;
using Enigmatry.Entry.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Enigmatry.Entry.AspNetCore.Exceptions;

internal class ExceptionHandler
{
    [SuppressMessage("ReSharper", "RedundantSuppressNullableWarningExpression",
        Justification = "If handler path feature could not be found it means error handling doesn't work!")]
    internal static async Task HandleExceptionFrom(HttpContext context)
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()!.Error;
        switch (exception)
        {
            case ValidationException validationException:
                await HandleValidationExceptionFrom(context, validationException);
                return;
            case EntityNotFoundException notFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            default:
                await HandleUnexpectedErrorFrom(context, exception);
                break;
        }
    }

    private static async Task HandleValidationExceptionFrom(HttpContext context,
        ValidationException validationException)
    {
        var validationResult = context.CreateValidationProblemDetails(validationException);
        var jsonResult = new JsonResult(validationResult)
        {
            ContentType = "application/problem+json",
            StatusCode = StatusCodes.Status400BadRequest
        };
        await ExecuteResult(context, jsonResult);
    }

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    private static async Task HandleUnexpectedErrorFrom(HttpContext context, Exception exception)
    {
        var accept = context.Request.GetTypedHeaders().Accept;
        if (accept != null && accept.All(header => header.MediaType != "application/json"))
        {
            // server does not accept Json, leaving to default MVC error page handler.
            return;
        }

        var problemDetails = GetProblemDetails(context, exception);
        var jsonResult = new JsonResult(problemDetails)
        {
            ContentType = "application/problem+json",
            StatusCode = 500
        };
        await ExecuteResult(context, jsonResult);
    }

    private static async Task ExecuteResult(HttpContext context, IActionResult actionResult)
    {
        RouteData routeData = context.GetRouteData();
        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(context, routeData, actionDescriptor);
        await actionResult.ExecuteResultAsync(actionContext);
    }

    private static ProblemDetails GetProblemDetails(HttpContext context, Exception exception)
    {
        var environment = context.Resolve<IHostEnvironment>();
        var errorDetail = environment.IsDevelopment()
            ? exception.Demystify().ToString()
            : "The instance value should be used to identify the problem when calling customer support";

        var problemDetails = new ProblemDetails
        {
            Title = "An unexpected error occurred!",
            Instance = context.Request.Path,
            Status = StatusCodes.Status500InternalServerError,
            Detail = errorDetail
        };

        return problemDetails;
    }
}
