using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Enigmatry.Blueprint.BuildingBlocks.Infrastructure.MediatR
{
    public class NullMediator : IMediator
    {
        public Task Publish(object notification, CancellationToken cancellationToken = new ()) =>
            Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification,
            CancellationToken cancellationToken = default) where TNotification : INotification =>
            Task.CompletedTask;

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(default(TResponse)!);

        public Task<object?> Send(object request, CancellationToken cancellationToken = new ()) =>
            Task.FromResult(default(object));
    }
}
