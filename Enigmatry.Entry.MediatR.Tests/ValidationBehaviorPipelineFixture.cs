using Enigmatry.Entry.MediatR.Tests.Samples;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Enigmatry.Entry.MediatR.Tests;

[Category("unit")]
public class ValidationBehaviorPipelineFixture
{
    private IServiceProvider _serviceProvider = null!;
    private IMediator _mediator = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = new TestServiceProviderBuilder().Build();
        _mediator = _serviceProvider.GetRequiredService<IMediator>();
    }

    [Test]
    public async Task TestValidRequestWithResponse()
    {
        var request = AValidRequestWithResponse();

        var response = await _mediator.Send(request, CancellationToken.None);

        await Verify(response);
    }

    [Test]
    public async Task TestInvalidRequestWithResponse()
    {
        var request = AnInvalidRequestWithResponse();

        ValidationException? validationException = null;
        try
        {
            _ = await _mediator.Send(request, CancellationToken.None);
        }
        catch (ValidationException e)
        {
            validationException = e;
        }

        await Verify(validationException);
    }

    [Test]
    public async Task TestValidRequestWithoutResponse()
    {
        var request = AValidRequestWithoutResponse();

        await _mediator.Send(request, CancellationToken.None);
    }

    [Test]
    public async Task TestInvalidRequestWithoutResponse()
    {
        var request = AnInvalidRequestWithoutResponse();

        ValidationException? validationException = null;
        try
        {
            await _mediator.Send(request, CancellationToken.None);
        }
        catch (ValidationException e)
        {
            validationException = e;
        }

        await Verify(validationException);
    }

    private static RequestWithResponse AValidRequestWithResponse() => new() { Name = "Some Name" };
    private static RequestWithResponse AnInvalidRequestWithResponse() => new() { Name = string.Empty };

    private static RequestWithoutResponse AValidRequestWithoutResponse() => new() { Name = "Some Name" };
    private static RequestWithoutResponse AnInvalidRequestWithoutResponse() => new() { Name = string.Empty };
}
