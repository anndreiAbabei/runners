using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Runners.Logging;

namespace Runners.Tests.Logging;

public sealed class ConsolePrintWithLoggingWriteTests
{
    [Fact]
    public void Should_LogInfo_Write()
    {
        // arrange
        var logger = new MockLogger();
        var logFactory = Substitute.For<ILoggerFactory>();
        logFactory.CreateLogger(ConsolePrintWithLogging.LogSource)
                  .Returns(logger);
        var sut = new ConsolePrintWithLogging(logFactory);
        const string text = "test";
        const string expectedLogMessage = $"Console Write Message: {text}";

        // act
        sut.Write(text);

        // assert
        Assert.Contains(logger.LoggedItems, item => item.Message == expectedLogMessage && 
                                                    item.LogLevel == LogLevel.Debug);
    }
    
    [Fact]
    public void Should_LogInfo_WriteLine()
    {
        // arrange
        var logger = new MockLogger();
        var logFactory = Substitute.For<ILoggerFactory>();
        logFactory.CreateLogger(ConsolePrintWithLogging.LogSource)
                  .Returns(logger);
        var sut = new ConsolePrintWithLogging(logFactory);
        const string text = "test";
        const string expectedLogMessage = $"Console WriteLine Message: {text}";

        // act
        sut.WriteLine(text);

        // assert
        Assert.Contains(logger.LoggedItems, item => item.Message == expectedLogMessage && 
                                                    item.LogLevel == LogLevel.Debug);
    }
    
    [ExcludeFromCodeCoverage]
    private sealed class MockLogger : ILogger
    {
        private readonly List<LogItem> _loggedItems = [];
        
        internal IEnumerable<LogItem> LoggedItems => _loggedItems;
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _loggedItems.Add(new LogItem(logLevel, formatter(state, exception)));
        }
        
        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    }
    
    [ExcludeFromCodeCoverage]
    private sealed record LogItem(LogLevel LogLevel, string Message);
}

