using Microsoft.Extensions.Logging;
using Runners.Logging;

namespace Runners.Tests.Logging;

public sealed class FileLoggerIsEnabledTests
{
    [Fact]
    public void Should_ReturnTrue_WhenLogLevelGreaterThanRequired()
    {
        // arrange
        var sut = new FileLogger("test", null!, LogLevel.Trace);
        
        // act
        var result = sut.IsEnabled(LogLevel.Warning);
        
        // assert
        Assert.True(result);
    }
    
    [Fact]
    public void Should_ReturnTrue_WhenLogLevelEqualThanRequired()
    {
        // arrange
        var sut = new FileLogger("test", null!, LogLevel.Warning);
        
        // act
        var result = sut.IsEnabled(LogLevel.Warning);
        
        // assert
        Assert.True(result);
    }
    
    [Fact]
    public void Should_ReturnFalse_WhenLogLevelLessThanRequired()
    {
        // arrange
        var sut = new FileLogger("test", null!, LogLevel.Warning);
        
        // act
        var result = sut.IsEnabled(LogLevel.Information);
        
        // assert
        Assert.False(result);
    }
}
