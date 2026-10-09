using MediatR;
using Microsoft.Extensions.Logging;

namespace Interbank.Productos.Comercial.Fcd.Application.Common
{

    public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

        public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            //Request
            _logger.LogInformation(
                "Handling {Name}. {@Date}",
                requestName,
                DateTime.UtcNow);

            var result = await next(cancellationToken);

            //Response
            _logger.LogInformation(
                "CleanArchitecture Request: {Name} {@Request}. {@Date}",
                requestName,
                request,
                DateTime.UtcNow);

            return result;
        }
    }
}
