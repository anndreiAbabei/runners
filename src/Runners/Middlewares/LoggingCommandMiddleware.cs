using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SlimeTools.Commander;

namespace Runners.Middlewares;

public sealed class LoggingCommandMiddleware : IMiddleware
{
    private readonly ILogger<LoggingCommandMiddleware> _logger;
    
    public LoggingCommandMiddleware(ILogger<LoggingCommandMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<Result> Handle(ICommandContext context, MiddlewareHandler next, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Start executing command {Command} using the {RawArgs} in directory {Directory}", 
                         context.Command.Name, context.RawArgs, context.RunningDirectory);
        
        return await next(context, cancellationToken);
    }
}
