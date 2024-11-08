using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Enigmatry.Entry.MediatR;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest).FullName;
        using (LogContext.PushProperty("MediatRRequestType", requestType))
        {
            logger.LogInformation("Handling {RequestType}", requestType);
            var response = await next();
            logger.LogInformation("Handled {RequestType}", requestType);
            return response;
        }
    }
}
