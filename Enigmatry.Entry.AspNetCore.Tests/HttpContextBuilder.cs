using Enigmatry.Entry.AspNetCore.Exceptions;
using FakeItEasy;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Enigmatry.Entry.AspNetCore.Tests;

internal class HttpContextBuilder
{
    private Exception _exception = null!;
    private string _accept = "application/json";

    private HttpContextBuilder() { }

    internal static HttpContextBuilder Create() => new();

    internal HttpContextBuilder With(Exception exception)
    {
        _exception = exception;
        return this;
    }

    internal HttpContextBuilder WithXmlRequest()
    {
        _accept = "application/xml";
        return this;
    }

    internal HttpContext Build()
    {
        var request = new HttpRequestFeature
        {
            QueryString = "request",
            Headers = new HeaderDictionary
            {
                { "Accept", _accept }
            },
            Body = new MemoryStream()
        };

        var stream = new MemoryStream();
        var response = new HttpResponseFeature
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Body = stream
        };

        var features = new FeatureCollection();
        features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(stream));
        features.Set<IHttpRequestFeature>(request);
        features.Set<IHttpResponseFeature>(response);
        features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature
        {
            Error = _exception ?? new InvalidOperationException("Something went wrong!")
        });

        var provider = new ServiceCollection()
            .AddTransient(_ => A.Fake<IHostEnvironment>())
            .AddTransient<ILogger<ExceptionHandler>>(_ => new NullLogger<ExceptionHandler>())
            .AddTransient<IActionResultExecutor<JsonResult>, SystemTextJsonResultActionExecutor>()
            .BuildServiceProvider();

        return new DefaultHttpContext(features)
        {
            RequestServices = provider
        };
    }

    internal class SystemTextJsonResultActionExecutor : IActionResultExecutor<JsonResult>
    {
        // Taken from Microsoft.AspNetCore.Mvc.Infrastructure.SystemTextJsonResultExecutor, trying to resemble Execute method
        public async Task ExecuteAsync(ActionContext context, JsonResult result)
        {
            var response = context.HttpContext.Response;
            response.ContentType = result.ContentType!;
            response.StatusCode = result.StatusCode.GetValueOrDefault();
            var value = result.Value;
            var objectType = value?.GetType() ?? typeof(object);
            var responseStream = response.Body;
            await JsonSerializer.SerializeAsync(responseStream, value, objectType);
            await responseStream.FlushAsync(context.HttpContext.RequestAborted);
        }
    }
}
