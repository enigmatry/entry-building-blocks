using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Enigmatry.Entry.AspNetCore.Validation;
using Enigmatry.Entry.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Enigmatry.Entry.AspNetCore.Filters
{
    public sealed class HandleExceptionsFilter : ExceptionFilterAttribute
    {
        private readonly bool _useDeveloperExceptionPage;

        public HandleExceptionsFilter(bool useDeveloperExceptionPage)
        {
            _useDeveloperExceptionPage = useDeveloperExceptionPage;
        }

        public override void OnException(ExceptionContext context)
        {
            ILogger<HandleExceptionsFilter> logger = context.HttpContext.Resolve<ILogger<HandleExceptionsFilter>>();
            if (context.Exception is ValidationException validationException)
            {
                logger.LogDebug(context.Exception, "Validation exception");
                context.Result = context.HttpContext.CreateValidationProblemDetailsResponse(validationException);
                return;
            }

            if (context.Exception is EntityNotFoundException exception)
            {
                logger.LogError(context.Exception, $"Entity: {exception.EntityName} not found");
                context.Result = new NotFoundResult();
                return;
            }

            logger.LogError(context.Exception, "Unexpected error");

            IList<MediaTypeHeaderValue> accept = context.HttpContext.Request.GetTypedHeaders().Accept;
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
            var errorDetail = _useDeveloperExceptionPage
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
    }
}
