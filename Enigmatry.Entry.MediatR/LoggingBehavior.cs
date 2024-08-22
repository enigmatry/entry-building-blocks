using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enigmatry.Entry.MediatR
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestType = typeof(TRequest).FullName;

            using (_logger.BeginScope(new Dictionary<string, object> { ["MediatRRequestType"] = requestType! }))
            {
                _logger.LogInformation("Handling {RequestType}", requestType);
                var response = await next();
                _logger.LogInformation("Handled {RequestType}", requestType);
                return response;
            }
        }
    }
}
