using Enigmatry.Entry.AspNetCore.Exceptions;
using FakeItEasy;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.Core.Tests;
internal class HttpContextBuilder
{
    private Exception _exception;
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
            .AddTransient(_ => A.Fake<ILogger<ExceptionHandler>>())
            .BuildServiceProvider();

        return new DefaultHttpContext(features)
        {
            RequestServices = provider
        };
    }
}
