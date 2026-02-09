using Runners.Extensions;
using Runners.Services;

namespace Runners.Tests.Extensions;

using Xunit;
using NSubstitute;
using System.Runtime.InteropServices;

public sealed class OsInfoProviderExIsWindowsTests
{
    [Fact]
    public void Should_ReturnTrue_WhenRunningOnWindows()
    {
        // Arrange
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Windows).Returns(true);

        // Act
        var result = provider.IsWindows;

        // Assert
        Assert.True(result);
        provider.Received(1).IsOSPlatform(OSPlatform.Windows);
    }

    [Fact]
    public void Should_ReturnFalse_WhenNotRunningOnWindows()
    {
        // Arrange
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Windows).Returns(false);

        // Act
        var result = provider.IsWindows;

        // Assert
        Assert.False(result);
        provider.Received(1).IsOSPlatform(OSPlatform.Windows);
    }
}