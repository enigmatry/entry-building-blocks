using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Enigmatry.BuildingBlocks.MediatR
{
    public class NullMediator : IMediator
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification,
            CancellationToken cancellationToken = default) where TNotification : INotification =>
            Task.CompletedTask;

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(default(TResponse)!);

        public Task<object?> Send(object request, CancellationToken cancellationToken = default) =>
            Task.FromResult(default(object));

#if NETSTANDARD2_1_OR_GREATER
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
            CancellationToken cancellationToken = new()) =>
            throw new System.NotImplementedException();

        public IAsyncEnumerable<object?> CreateStream(object request,
            CancellationToken cancellationToken = new()) =>
            throw new System.NotImplementedException();
#endif
    }
}
