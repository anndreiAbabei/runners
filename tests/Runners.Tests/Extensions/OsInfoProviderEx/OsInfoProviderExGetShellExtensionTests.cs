using System.Runtime.InteropServices;
using NSubstitute;
using Runners.Extensions;
using Runners.Services;

namespace Runners.Tests.Extensions;

public sealed class OsInfoProviderExGetShellExtensionTests
{
    [Fact]
    public void Should_ReturnCmd_WhenRunningOnWindows()
    {
        // Arrange
        const string expectedShell = "cmd";
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Windows).Returns(true);

        // Act
        var result = provider.GetShellExtension();

        // Assert
        Assert.Equal(expectedShell, result);
        provider.Received(1).IsOSPlatform(OSPlatform.Windows);
    }

    [Fact]
    public void Should_ReturnSh_WhenRunningOnMacOs()
    {
        // Arrange
        const string expectedShell = "sh";
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Windows).Returns(false);

        // Act
        var result = provider.GetShellExtension();

        // Assert
        Assert.Equal(expectedShell, result);
        provider.Received(1).IsOSPlatform(OSPlatform.Windows);
    }

    [Fact]
    public void Should_ReturnSh_WhenRunningOnLinux()
    {
        // Arrange
        const string expectedShell = "sh";
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Windows).Returns(false);

        // Act
        var result = provider.GetShellExtension();

        // Assert
        Assert.Equal(expectedShell, result);
        provider.Received(1).IsOSPlatform(OSPlatform.Windows);
    }
}
