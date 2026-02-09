using NSubstitute;
using Runners.Services;

namespace Runners.Tests.Services;

public sealed class CommandProviderCreateTests
{
    [Fact]
    public void Should_CreateCommand()
    {
        // arrange
        const string expectedCommand = "git";
        var runtimeProvider = Substitute.For<IRuntimeInformationProvider>();
        var sut = new CommandProvider(runtimeProvider);

        // act
        var command = sut.Create(expectedCommand);

        // assert
        Assert.Equal(expectedCommand, command.TargetFilePath);
    }
}
