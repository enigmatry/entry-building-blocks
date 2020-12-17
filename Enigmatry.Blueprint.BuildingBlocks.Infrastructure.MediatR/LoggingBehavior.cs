using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Enigmatry.Blueprint.BuildingBlocks.Infrastructure.MediatR
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var requestType = typeof(TRequest).FullName;
            using (LogContext.PushProperty("MediatRRequestType", requestType))
            {
                _logger.LogInformation("Handling {RequestType}", requestType);
                var response = await next();
                _logger.LogInformation("Handled {RequestType}", requestType);
                return response;
            }
        }
    }
}
