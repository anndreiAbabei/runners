using Microsoft.Extensions.Logging;
using Runners.Logging;

namespace Runners.Tests.Logging;

public sealed class FileLoggingProviderCreateLoggerTests
{
    [Fact]
    public void Should_CreateLogger()
    {
        // arrange
        using var sut = new FileLoggingProvider($"test.{Guid.NewGuid():N}.log", LogLevel.Critical);
        
        // act
        var logger = sut.CreateLogger("test");

        // assert
        _ = Assert.IsType<FileLogger>(logger);
    }
}
