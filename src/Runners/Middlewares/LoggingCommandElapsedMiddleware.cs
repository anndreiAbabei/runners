using System.Diagnostics;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SlimeTools.Commander;

namespace Runners.Middlewares;

public sealed class LoggingCommandElapsedMiddleware : IMiddleware
{
    private readonly ILogger<LoggingCommandElapsedMiddleware> _logger;
    
    public LoggingCommandElapsedMiddleware(ILogger<LoggingCommandElapsedMiddleware> logger)
    {
        _logger = logger;
    }    
    
    public async ValueTask<Result> Handle(ICommandContext context, MiddlewareHandler next, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        
        var result = await next(context, cancellationToken);
        
        sw.Stop();
        
        _logger.LogDebug("Command {Command} executed in {Elapsed}", context.Command, sw.Elapsed);
        
        return result;
    }
}
