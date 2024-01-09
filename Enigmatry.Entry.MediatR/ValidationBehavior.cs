using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Enigmatry.Entry.MediatR
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseRequest
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var results = await Task.WhenAll(_validators
                .Select(async v => await v.ValidateAsync(request, cancellationToken)));

            var failures = results
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            return failures.Count != 0 ? throw new ValidationException(failures) : await next();
        }
    }
}
