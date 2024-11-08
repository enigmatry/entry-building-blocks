using MediatR;

namespace Enigmatry.Entry.MediatR;

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

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = new())
        where TRequest : IRequest => Task.CompletedTask;

    public Task<object?> Send(object request, CancellationToken cancellationToken = default) =>
        Task.FromResult(default(object));

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();

    public IAsyncEnumerable<object?> CreateStream(object request,
        CancellationToken cancellationToken = new()) =>
        throw new NotImplementedException();
}
