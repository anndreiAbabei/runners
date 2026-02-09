using System.Runtime.InteropServices;
using NSubstitute;
using Runners.Extensions;
using Runners.Services;

namespace Runners.Tests.Extensions;

public sealed class OsInfoProviderExIsLinuxTests
{
    [Fact]
    public void Should_ReturnTrue_WhenRunningOnLinux()
    {
        // Arrange
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Linux).Returns(true);

        // Act
        var result = provider.IsLinux;

        // Assert
        Assert.True(result);
        provider.Received(1).IsOSPlatform(OSPlatform.Linux);
    }

    [Fact]
    public void Should_ReturnFalse_WhenNotRunningOnLinux()
    {
        // Arrange
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Linux).Returns(false);

        // Act
        var result = provider.IsLinux;

        // Assert
        Assert.False(result);
        provider.Received(1).IsOSPlatform(OSPlatform.Linux);
    }
}
