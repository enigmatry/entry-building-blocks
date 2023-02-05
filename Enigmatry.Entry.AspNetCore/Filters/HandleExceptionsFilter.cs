using Enigmatry.Entry.AspNetCore.Validation;
using Enigmatry.Entry.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Enigmatry.Entry.AspNetCore.Filters;

[Obsolete("ExceptionsFilter is deprecated due to limited context, please use extensions method instead since more exceptions can be caught from it.")]
public class HandleExceptionsFilter : IAsyncExceptionFilter, IExceptionFilter
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<HandleExceptionsFilter> _logger;

    public HandleExceptionsFilter(IHostEnvironment hostEnvironment, ILogger<HandleExceptionsFilter> logger)
    {
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnException(ExceptionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var handledElsewhere = OnBeforeException(context);
        if (handledElsewhere)
        {
            return;
        }

        switch (context.Exception)
        {
            case ValidationException validationException:
                _logger.LogDebug(context.Exception, "Validation exception");
                context.Result = context.HttpContext.CreateValidationProblemDetailsResponse(validationException);
                return;
            case EntityNotFoundException exception:
                _logger.LogError(context.Exception, $"Entity: {exception.EntityName} not found");
                context.Result = new NotFoundResult();
                return;
            default:
                _logger.LogError(context.Exception, "Unexpected error");
                HandleUnexpectedErrorFrom(context);
                break;
        }
    }

    protected virtual bool OnBeforeException(ExceptionContext context) => false;

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    private void HandleUnexpectedErrorFrom(ExceptionContext context)
    {
        var accept = context.HttpContext.Request.GetTypedHeaders().Accept;
        if (accept != null && accept.All(header => header.MediaType != "application/json"))
        {
            // server does not accept Json, leaving to default MVC error page handler.
            return;
        }

        var jsonResult = new JsonResult(GetProblemDetails(context))
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            ContentType = "application/problem+json"
        };
        context.Result = jsonResult;
    }

    private ProblemDetails GetProblemDetails(ExceptionContext context)
    {
        var errorDetail = _hostEnvironment.IsDevelopment()
            ? context.Exception.Demystify().ToString()
            : "The instance value should be used to identify the problem when calling customer support";

        var problemDetails = new ProblemDetails
        {
            Title = "An unexpected error occurred!",
            Instance = context.HttpContext.Request.Path,
            Status = StatusCodes.Status500InternalServerError,
            Detail = errorDetail
        };

        return problemDetails;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        OnException(context);
        return Task.CompletedTask;
    }
}
