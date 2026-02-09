using System.Runtime.InteropServices;
using NSubstitute;
using Runners.Extensions;
using Runners.Services;

namespace Runners.Tests.Extensions;

public sealed class OsInfoProviderExIsMacOsTests
{
    [Fact]
    public void Should_ReturnTrue_WhenRunningOnMacOs()
    {
        // Arrange
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.OSX).Returns(true);

        // Act
        var result = provider.IsMacOs;

        // Assert
        Assert.True(result);
        provider.Received(1).IsOSPlatform(OSPlatform.OSX);
    }

    [Fact]
    public void Should_ReturnFalse_WhenNotRunningOnMacOs()
    {
        // Arrange
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.OSX).Returns(false);

        // Act
        var result = provider.IsMacOs;

        // Assert
        Assert.False(result);
        provider.Received(1).IsOSPlatform(OSPlatform.OSX);
    }
}
