using FluentValidation;
using MediatR;

namespace Enigmatry.Entry.MediatR;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var results = await Task.WhenAll(validators
            .Select(async v => await v.ValidateAsync(request, cancellationToken)));

        var failures = results
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        return failures.Count != 0 ? throw new ValidationException(failures) : await next(cancellationToken);
    }
}
