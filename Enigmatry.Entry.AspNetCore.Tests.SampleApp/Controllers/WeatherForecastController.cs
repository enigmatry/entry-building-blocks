using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

#pragma warning disable CA2201
#pragma warning disable IDE0055
#pragma warning disable CA5394
#pragma warning disable IDE0052

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get() =>
        Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

    [HttpGet("throwsError")]
    public IEnumerable<WeatherForecast> ThrowsError()
    {
        throw new Exception("Some exception");
    }

    [HttpGet("problemDetails")]
    public IEnumerable<WeatherForecast> ProblemDetails() =>
        throw new ValidationException("AValidationExceptionMessage",
            new List<ValidationFailure>() { new ValidationFailure("AProperty", "AFailedValidationMessage") });
}
