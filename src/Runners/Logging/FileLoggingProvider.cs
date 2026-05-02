using Microsoft.Extensions.Logging;

namespace Runners.Logging;

public sealed class FileLoggingProvider : ILoggerProvider
{
    private readonly TextWriter _textWriter;
    private readonly LogLevel _minimumLogLevel;
    
    public FileLoggingProvider(string filePath, LogLevel minimumLogLevel)
    {
        _textWriter = new StreamWriter(filePath)
        {
            AutoFlush = true
        };
        _minimumLogLevel = minimumLogLevel;
    }
    
    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _textWriter, _minimumLogLevel);
    
    public void Dispose() => _textWriter.Dispose();
}

internal sealed class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly TextWriter _writer;
    private readonly LogLevel _minimumLogLevel;
    
    public FileLogger(string categoryName, TextWriter writer, LogLevel minimumLogLevel)
    {
        _categoryName = categoryName;
        _writer = writer;
        _minimumLogLevel = minimumLogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if(!IsEnabled(logLevel))
            return;
        
        var message = formatter(state, exception);

        _writer.WriteLine($"[{logLevel}] [{_categoryName}] {message}");
    }
    
    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLogLevel;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}