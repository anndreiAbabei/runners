using System.Runtime.InteropServices;
using NSubstitute;
using Runners.Services;
using Runners.Services.Commands;

namespace Runners.Tests.Services;

public sealed class CommandProviderShellTests
{
    [Fact]
    public void Should_ReturnCmd_WhenRunningOnWindows()
    {
        // arrange
        const string expectedCommand = "cmd";
        var runtimeProvider = Substitute.For<IRuntimeInformationProvider>();
        runtimeProvider.IsOSPlatform(OSPlatform.Windows).Returns(true);
        var sut = new CommandProvider(runtimeProvider);

        // act
        var command = sut.Shell();

        // assert
        Assert.Equal(expectedCommand, command.TargetFilePath);
        runtimeProvider.Received(1).IsOSPlatform(OSPlatform.Windows);
    }

    [Fact]
    public void Should_ReturnSh_WhenNotRunningOnWindows()
    {
        // arrange
        const string expectedCommand = "bash";
        var runtimeProvider = Substitute.For<IRuntimeInformationProvider>();
        runtimeProvider.IsOSPlatform(OSPlatform.Windows).Returns(false);
        var sut = new CommandProvider(runtimeProvider);

        // act
        var command = sut.Shell();

        // assert
        Assert.Equal(expectedCommand, command.TargetFilePath);
        runtimeProvider.Received(1).IsOSPlatform(OSPlatform.Windows);
    }
}
