using Microsoft.Extensions.Logging;

namespace Runners.Logging;

public sealed class FileLoggingProvider : ILoggerProvider
{
    private readonly StreamWriter _streamWriter;
    private readonly LogLevel _minimumLogLevel;
    
    public FileLoggingProvider(string filePath, LogLevel minimumLogLevel)
    {
        _streamWriter = new StreamWriter(filePath)
        {
            AutoFlush = true
        };
        _minimumLogLevel = minimumLogLevel;

    }
    
    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _streamWriter, _minimumLogLevel);
    
    public void Dispose() => _streamWriter.Dispose();
}

internal sealed class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly StreamWriter _streamWriter;
    private readonly LogLevel _minimumLogLevel;
    public FileLogger(string categoryName, StreamWriter streamWriter, LogLevel minimumLogLevel)
    {
        _categoryName = categoryName;
        _streamWriter = streamWriter;
        _minimumLogLevel = minimumLogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if(!IsEnabled(logLevel))
            return;
        
        var message = formatter(state, exception);

        _streamWriter.WriteLine($"[{logLevel}] [{_categoryName}] {message}");
    }
    
    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLogLevel;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}