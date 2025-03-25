using System.Text.Json;
using Enigmatry.Entry.AspNetCore.Exceptions;
using Enigmatry.Entry.Core.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Enigmatry.Entry.AspNetCore.Tests;

[Category("unit")]
public class ExceptionHandlerFixture
{
    [Test]
    public async Task EntityNotFoundTest()
    {
        var context = HttpContextBuilder.Create()
            .With(new EntityNotFoundException("myEntity", "MyEntity was not found!"))
            .Build();

        await HandleExceptionFrom(context);

        StatusCodeShouldBe(StatusCodes.Status404NotFound, context);
    }

    [Test]
    public async Task ValidationExceptionTest()
    {
        const string propertyName = "name";
        const string propertyError = "Name is required.";
        var context = HttpContextBuilder.Create()
            .With(new ValidationException(new List<ValidationFailure>
            {
                new(propertyName, propertyError)
            }))
            .Build();

        await HandleExceptionFrom(context);

        var result = await GetErrorsFrom<ValidationProblemDetails>(context)!;
        var errors = result!.Errors;
        errors.Count.ShouldBe(1);
        errors.First().Key.ShouldBe(propertyName);
        errors.First().Value.First().ShouldBe(propertyError);
    }

    [Test]
    public async Task XmlRequestTest()
    {
        var context = HttpContextBuilder.Create()
            .WithXmlRequest()
            .Build();

        await HandleExceptionFrom(context);

        (await GetResponseStringFrom(context)).ShouldBe(string.Empty);
    }

    [Test]
    public async Task JsonRequestTest()
    {
        var context = HttpContextBuilder.Create()
            .Build();

        await HandleExceptionFrom(context);

        StatusCodeShouldBe(StatusCodes.Status500InternalServerError, context);
        context.Response.ContentType.ShouldBe("application/problem+json");
        (await GetErrorsFrom<ProblemDetails>(context)).Title.ShouldBe("An unexpected error occurred!");
    }

    private static void StatusCodeShouldBe(int code, HttpContext context) => context.Response.StatusCode.ShouldBe(code);

    private static async Task HandleExceptionFrom(HttpContext context)
    {
        await ExceptionHandler.HandleExceptionFrom(context);
        context.Response.Body.Position = 0;
    }

    private static async Task<string> GetResponseStringFrom(HttpContext context)
    {
        using var streamReader = new StreamReader(context.Response.Body);
        return await streamReader.ReadToEndAsync();
    }

    private static async Task<T> GetErrorsFrom<T>(HttpContext context)
    {
        var value = await GetResponseStringFrom(context);
        return JsonSerializer.Deserialize<T>(value)!;
    }
}
