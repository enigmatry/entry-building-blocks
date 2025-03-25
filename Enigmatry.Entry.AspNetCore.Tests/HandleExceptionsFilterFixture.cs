using Enigmatry.Entry.AspNetCore.Filters;
using Enigmatry.Entry.Core.Entities;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Enigmatry.Entry.AspNetCore.Tests;

[Category("unit")]
[Obsolete("ExceptionsFilter is deprecated due to limited context, please use extensions method instead since more exceptions can be caught from it.")]
public class HandleExceptionsFilterFixture
{
    private const string GeneralException = "General exception";
    private HandleExceptionsFilter _filter = null!;

    [SetUp]
    public void Setup() => _filter = InitializeFilter();

    [Test]
    public void ValidationExceptionTest()
    {
        var context = ExceptionContextBuilder.Create()
                                             .WithValidationException()
                                             .Build();

        _filter.OnException(context);

        var errors = context.Result!.GetErrorMessages().ToList();

        errors.Count.ShouldBe(1);
        errors.First().ShouldBe("Name is required.");
    }

    [Test]
    public void EntityNotFoundExceptionTest()
    {
        const string errorMessage = "MyEntity was not found!";
        var context = ExceptionContextBuilder.Create()
            .WithCustomException(new EntityNotFoundException("myEntity", errorMessage))
            .Build();

        _filter.OnException(context);

        context.Result.ShouldBeOfType<NotFoundResult>();
        context.Exception.Message.ShouldBe(errorMessage);
    }

    [Test]
    public void XmlRequestTest()
    {
        var context = ExceptionContextBuilder.Create()
                                             .WithException(GeneralException)
                                             .WithXmlRequest()
                                             .Build();

        _filter.OnException(context);

        context.Result.ShouldBeNull();
    }

    [Test]
    public void JsonRequestTest()
    {
        const string contentType = "application/problem+json";
        var context = ExceptionContextBuilder.Create()
                                             .WithException(GeneralException)
                                             .WithJsonRequest()
                                             .Build();

        _filter.OnException(context);

        var result = (JsonResult)context.Result!;
        result.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        result.ContentType.ShouldBe(contentType);
    }

    private static HandleExceptionsFilter InitializeFilter(bool devModeOn = false)
    {
        var environment = A.Fake<IHostEnvironment>();
        A.CallTo(() => environment.EnvironmentName).Returns(devModeOn ? "Development" : "Production");

        var logger = A.Fake<ILogger<HandleExceptionsFilter>>();
        return new HandleExceptionsFilter(environment, logger);
    }
}
