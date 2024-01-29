using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enigmatry.Entry.AspNetCore.Tests.SampleApp.Authorization;
using Enigmatry.Entry.Core.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Enigmatry.Entry.AspNetCore.Tests.SampleApp.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IMediator _mediator;

    public WeatherForecastController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get() =>
        Enumerable.Range(0, Summaries.Length).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = index * 10,
            Summary = Summaries[index]
        }).ToArray();

    [HttpGet("throwsError")]
    public IEnumerable<WeatherForecast> ThrowsError() => throw new InvalidOperationException("Some exception");

    [HttpGet("throwsEntityNotFoundException")]
    public IEnumerable<WeatherForecast> ThrowsEntityNotFoundException() =>
        throw new EntityNotFoundException("Entity not found");

    [HttpGet("problemDetails")]
    public IEnumerable<WeatherForecast> ProblemDetails() =>
        throw new ValidationException("AValidationExceptionMessage",
            new List<ValidationFailure> { new("AProperty", "AFailedValidationMessage") });

    [HttpPost]
    public async Task UpdateWeatherForecast([FromBody] UpdateWeatherForecast.Request request) =>
        await _mediator.Send(request);

    [HttpGet("UserWithPermissionIsAllowed")]
    [AppAuthorize(PermissionId.Read, PermissionId.Write)]
    public IEnumerable<WeatherForecast> UserWithPermissionIsAllowed() => Array.Empty<WeatherForecast>();

    [HttpGet("userNoPermissionIsNotAllowed")]
    [AppAuthorize(PermissionId.Write)]
    public IEnumerable<WeatherForecast> UserNoPermissionIsNotAllowed() => Array.Empty<WeatherForecast>();

    [HttpGet("noAuthorizeAttribute")]
    public IEnumerable<WeatherForecast> NoAuthorizeAttribute() => Array.Empty<WeatherForecast>();

    [HttpGet("allowAnonymousAttribute")]
    [AllowAnonymous]
    public IEnumerable<WeatherForecast> AllowAnonymousAttribute() => Array.Empty<WeatherForecast>();
}
