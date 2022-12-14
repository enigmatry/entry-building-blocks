using Enigmatry.Entry.AspNetCore.Filters;
using FakeItEasy;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.Core.Tests;

internal class ExceptionContextBuilder
{
    private const string PropertyName = "name";
    private const string ErrorMessage = "Name is required.";
    [Obsolete]
    private readonly ILogger _logger = A.Fake<ILogger<HandleExceptionsFilter>>();
    private Exception _exception = default!;
    private bool _jsonRequest;
    private bool _xmlRequest;

    private ExceptionContextBuilder() { }

    internal static ExceptionContextBuilder Create() => new();

    internal ExceptionContextBuilder WithValidationException()
    {
        _exception = new ValidationException(new List<ValidationFailure>
            {
                new(PropertyName, ErrorMessage)
            });
        return this;
    }

    internal ExceptionContextBuilder WithCustomException<T>(T exception) where T : Exception
    {
        _exception = exception;
        return this;
    }

    internal ExceptionContextBuilder WithException(string message)
    {
        _exception = new InvalidOperationException(message);
        return this;
    }

    internal ExceptionContextBuilder WithJsonRequest()
    {
        _jsonRequest = true;
        return this;
    }

    internal ExceptionContextBuilder WithXmlRequest()
    {
        _xmlRequest = true;
        return this;
    }

    [Obsolete("ExceptionsFilter is deprecated due to limited context, please use extensions method instead since more exceptions can be caught from it.")]
    internal ExceptionContext Build()
    {
        var httpContext = A.Fake<HttpContext>();
        IList<IFilterMetadata> filters = new List<IFilterMetadata>();

        if (_jsonRequest)
        {
            ConfigureJsonHeaders(httpContext);
        }
        else if (_xmlRequest)
        {
            ConfigureXmlHeaders(httpContext);
        }

        ConfigureServiceProvider(httpContext);

        var actionContext = CreateActionContext(httpContext);

        var context = new ExceptionContext(actionContext, filters)
        {
            Exception = _exception
        };

        return context;
    }

    private static ActionContext CreateActionContext(HttpContext httpContext) =>
        new(
            httpContext,
            A.Fake<RouteData>(),
            A.Fake<ActionDescriptor>());

    [Obsolete]
    private void ConfigureServiceProvider(HttpContext httpContext)
    {
        var serviceProvider = A.Fake<IServiceProvider>();
        _ = A.CallTo(() => serviceProvider.GetService(A<Type>.Ignored)).Returns(_logger);
        _ = A.CallTo(() => httpContext.RequestServices).Returns(serviceProvider);
    }

    private static void ConfigureHeaders(HttpContext httpContext, string mimeType)
    {
        var request = A.Fake<HttpRequest>();
        _ = A.CallTo(() => request.Headers).Returns(new HeaderDictionary { { "accept", mimeType } });
        _ = A.CallTo(() => httpContext.Request).Returns(request);
    }

    private static void ConfigureJsonHeaders(HttpContext httpContext) => ConfigureHeaders(httpContext, "application/json");

    private static void ConfigureXmlHeaders(HttpContext httpContext) => ConfigureHeaders(httpContext, "text/xml");
}
