using Microsoft.Extensions.Logging;
using Runners.Logging;

namespace Runners.Tests.Logging;

public sealed class FileLoggerBeginScopeTests
{
    [Fact]
    public void Should_ReturnNull()
    {
        // arrange
        var sut = new FileLogger("test.txt", null!, LogLevel.Critical);

        // act
        var result = sut.BeginScope("test");

        // assert
        Assert.Null(result);
    }
}
