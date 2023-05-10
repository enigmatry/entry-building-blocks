using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Enigmatry.Entry.AspNetCore.Authorization.Attributes;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet]
    [SuppressMessage("Security", "CA5394:Do not use insecure randomness",
        Justification = "Suppressed until random utilities are merged.")]
    public IEnumerable<WeatherForecast> Get() =>
        Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
            .ToArray();

    [HttpGet("throwsError")]
    public IEnumerable<WeatherForecast> ThrowsError() => throw new InvalidOperationException("Some exception");

    [HttpGet("problemDetails")]
    public IEnumerable<WeatherForecast> ProblemDetails() =>
        throw new ValidationException("AValidationExceptionMessage",
            new List<ValidationFailure> { new("AProperty", "AFailedValidationMessage") });

    [HttpGet("userInRoleIsAllowed")]
    [UserHasRole("Admin,Tester")]
    public IEnumerable<WeatherForecast> UserInRoleIsAllowed() => Array.Empty<WeatherForecast>();

    [HttpGet("userNotInRoleIsNotAllowed")]
    [UserHasRole("Admin")]
    public IEnumerable<WeatherForecast> UserNotInRoleIsNotAllowed() => Array.Empty<WeatherForecast>();

    [HttpGet("UserWithPermissionIsAllowed")]
    [UserHasPermission("Read,Write")]
    public IEnumerable<WeatherForecast> UserWithPermissionIsAllowed() => Array.Empty<WeatherForecast>();

    [HttpGet("userNoPermissionIsNotAllowed")]
    [UserHasPermission("Write")]
    public IEnumerable<WeatherForecast> UserNoPermissionIsNotAllowed() => Array.Empty<WeatherForecast>();

    [HttpGet("unprotected")]
    public IEnumerable<WeatherForecast> Unprotected() => Array.Empty<WeatherForecast>();
}
