using System.Runtime.InteropServices;
using NSubstitute;
using Runners.Extensions;
using Runners.Services;

namespace Runners.Tests.Extensions.FileLocationProviderEx;

public sealed class FileLocationProviderExGetStateDirTests
{
    [Fact]
    public void Should_ReturnLinuxPath()
    {
        // arrange
        const string expectedFolder = $"/var/lib/{Constants.SafeAppName}";
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Linux).Returns(true);
        var fm = Substitute.For<IFileSystemManager>();

        // act
        var result = provider.GetStateDir(fm);

        // assert
        Assert.Equal(expectedFolder, result);
    }
    
    [Fact]
    public void Should_ReturnFreeBsdPath()
    {
        // arrange
        const string expectedFolder = $"/var/db/{Constants.SafeAppName}";
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.FreeBSD).Returns(true);
        var fm = Substitute.For<IFileSystemManager>();

        // act
        var result = provider.GetStateDir(fm);

        // assert
        Assert.Equal(expectedFolder, result);
    }
    
    [Fact]
    public void Should_ReturnMacOsPath()
    {
        // arrange
        const string expectedFolder = $"/Library/Application Support/{Constants.SafeAppName}";
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.OSX).Returns(true);
        var fm = Substitute.For<IFileSystemManager>();

        // act
        var result = provider.GetStateDir(fm);

        // assert
        Assert.Equal(expectedFolder, result);
    }
    
    [Fact]
    public void Should_ReturnWindowsPath()
    {
        // arrange
        var expectedFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.SafeAppName);
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsOSPlatform(OSPlatform.Windows).Returns(true);
        var fm = Substitute.For<IFileSystemManager>();

        // act
        var result = provider.GetStateDir(fm);

        // assert
        Assert.Equal(expectedFolder, result);
    }
    
    [Fact]
    public void Should_ReturnDebugPath()
    {
        // arrange
        var expectedFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Constants.SafeAppName);
        var provider = Substitute.For<IRuntimeInformationProvider>();
        provider.IsDebug.Returns(true);
        var fm = Substitute.For<IFileSystemManager>();

        // act
        var result = provider.GetStateDir(fm);

        // assert
        Assert.Equal(expectedFolder, result);
    }
}
