using Microsoft.Extensions.Logging;
using SlimeTools.Commander;

namespace Runners.Logging;

public sealed class ConsolePrintWithLogging : IConsolePrint
{
    internal const string LogSource = nameof(IConsolePrint);
    
    private readonly ILogger _logger;
    
    public ConsolePrintWithLogging(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger(LogSource);
    }

    public void Write(string message)
    {
        _logger.LogDebug("Console Write Message: {Message}", message);
        Console.Write(message);
    }
    
    public void WriteLine(string message)
    {
        _logger.LogDebug("Console WriteLine Message: {Message}", message);
        Console.WriteLine(message);
    }
}
