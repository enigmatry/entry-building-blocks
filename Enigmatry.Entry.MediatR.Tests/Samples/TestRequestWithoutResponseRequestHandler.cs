using JetBrains.Annotations;
using MediatR;

namespace Enigmatry.Entry.MediatR.Tests.Samples;

[UsedImplicitly]
public class TestRequestWithoutResponseRequestHandler : IRequestHandler<RequestWithoutResponse>
{
    public Task Handle(RequestWithoutResponse request, CancellationToken cancellationToken) => Task.CompletedTask;
}
