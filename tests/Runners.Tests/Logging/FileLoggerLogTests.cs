using Microsoft.Extensions.Logging;
using NSubstitute;
using Runners.Logging;

namespace Runners.Tests.Logging;

public sealed class FileLoggerLogTests
{
    [Fact]
    public void Should_WriteLog_WhenLogLevelGreaterThanRequired()
    {
        // arrange
        const string expectedCategory = "test-cat";
        const string expectedMessage = "test-message";
        const LogLevel expectedLevel = LogLevel.Information;
        var writer = Substitute.For<TextWriter>();
        var sut = new FileLogger(expectedCategory, writer, LogLevel.Debug);

        // act
        sut.Log(expectedLevel, expectedMessage);

        // assert
        writer.Received().WriteLine($"[{expectedLevel}] [{expectedCategory}] {expectedMessage}");
    }
    
    [Fact]
    public void Should_WriteLog_WhenLogLevelEqualWithRequired()
    {
        // arrange
        const string expectedCategory = "test-cat";
        const string expectedMessage = "test-message";
        const LogLevel expectedLevel = LogLevel.Information;
        var writer = Substitute.For<TextWriter>();
        var sut = new FileLogger(expectedCategory, writer, expectedLevel);

        // act
        sut.Log(expectedLevel, expectedMessage);

        // assert
        writer.Received().WriteLine($"[{expectedLevel}] [{expectedCategory}] {expectedMessage}");
    }
    
    [Fact]
    public void Should_NotWriteLog_WhenLogLevelSmallerThanRequired()
    {
        // arrange
        const string expectedCategory = "test-cat";
        const string expectedMessage = "test-message";
        const LogLevel expectedLevel = LogLevel.Information;
        var writer = Substitute.For<TextWriter>();
        var sut = new FileLogger(expectedCategory, writer, LogLevel.Error);

        // act
        sut.Log(expectedLevel, expectedMessage);

        // assert
        writer.DidNotReceive().WriteLine($"[{expectedLevel}] [{expectedCategory}] {expectedMessage}");
    }
}
