using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Enigmatry.Entry.AspNetCore.Validation;

public static class ActionContextExtensions
{
    public static BadRequestObjectResult CreateValidationProblemDetailsResponse(this HttpContext context, ModelStateDictionary modelState)
    {
        ValidationProblemDetails problemDetails = CreateValidationProblemDetails(context, modelState);
        return ToBadRequestObjectResult(problemDetails);
    }

    public static BadRequestObjectResult CreateValidationProblemDetailsResponse(this HttpContext context, ValidationException validationException)
    {
        ValidationProblemDetails problemDetails = CreateValidationProblemDetails(context);
        CopyErrorsFromValidationException(problemDetails, validationException.Errors);
        return ToBadRequestObjectResult(problemDetails);
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(HttpContext context,
        ModelStateDictionary? modelState = null)
    {
        ValidationProblemDetails details = modelState != null ?
            new ValidationProblemDetails(modelState) :
            new ValidationProblemDetails
            {
                Instance = context.Request.Path,
                Status = StatusCodes.Status400BadRequest,
                Type = "https://asp.net/core",
                Detail = "Please refer to the errors property for additional details."
            };
        return details;
    }

    private static BadRequestObjectResult ToBadRequestObjectResult(ValidationProblemDetails problemDetails) =>
        new(problemDetails)
        {
            ContentTypes = { "application/problem+json", "application/problem+xml" }
        };

    private static void CopyErrorsFromValidationException(ValidationProblemDetails problemDetails, IEnumerable<ValidationFailure> validationExceptionErrors)
    {
        foreach (ValidationFailure validationExceptionError in validationExceptionErrors)
        {
            var key = validationExceptionError.PropertyName;
            if (!problemDetails.Errors.TryGetValue(key, out var messages))
            {
                messages = Array.Empty<string>();
            }

            messages = messages.Concat(new[] { validationExceptionError.ErrorMessage }).ToArray();
            problemDetails.Errors[key] = messages;
        }
    }
}
