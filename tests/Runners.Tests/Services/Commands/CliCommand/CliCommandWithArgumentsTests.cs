using Runners.Services.Commands;

namespace Runners.Tests.Services.Commands;

public sealed class CliCommandWithArgumentsTests
{
    [Fact]
    public void Should_SetArguments()
    {
        // arrange
        var expectedArguments = new[] { "arg1", "arg2" };
        var sut = new CliCommand("test");

        // act
        sut.WithArguments(expectedArguments);

        // assert
        Assert.Equal(expectedArguments, sut.Arguments);
    }
}
