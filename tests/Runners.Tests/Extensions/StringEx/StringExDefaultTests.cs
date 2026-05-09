using Runners.Extensions;

namespace Runners.Tests.Extensions;

public sealed class StringExDefaultTests
{
    [Fact]
    public void Should_ReturnString_WhenIsNotEmpty()
    {
        // arrange
        const string value = "Test";
        
        // act
        var result = value.Default("INVALID");

        // assert
        Assert.Equal(value, result);
    }
    
    [Fact]
    public void Should_ReturnDefaultString_WhenIsEmpty()
    {
        // arrange
        const string value = "";
        const string expectedValue = "Test";
        
        // act
        var result = value.Default(expectedValue);

        // assert
        Assert.Equal(expectedValue, result);
    }
}
