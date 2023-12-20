using JetBrains.Annotations;
using MediatR;

namespace Enigmatry.Entry.MediatR.Tests.Samples;

[UsedImplicitly]
public class TestRequestWithResponseRequestHandler : IRequestHandler<RequestWithResponse, Response>
{
    public Task<Response> Handle(RequestWithResponse request, CancellationToken cancellationToken)
    {
        var response = new Response { Handled = "Yes" };
        return Task.FromResult(response);
    }
}
