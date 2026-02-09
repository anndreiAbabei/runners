using System.Runtime.InteropServices;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Runners.Services;
using Runners.Services.Commands;

namespace Runners.Tests.Services;

public sealed class RunnerManagerGetRunnerFolderTests
{
    [Fact]
    public void Should_ReturnSuccess_WithFolderName()
    {
        // arrange
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        runtime.IsOSPlatform(OSPlatform.Linux).Returns(true);
        var cmd = Substitute.For<ICommandProvider>();
        var fm = Substitute.For<IFileSystemManager>();
        var log = Substitute.For<ILogger<RunnerManager>>();
        var sut = new RunnerManager(runtime, cmd, fm, log);
        const string name = "test-runner";
        const string expectedFolder = $"/var/lib/{Constants.SafeAppName}/runners/{name}";
        
        // act
        var (isSuccess, _, value) = sut.GetRunnerFolder(name);

        // assert
        Assert.True(isSuccess);
        Assert.Equal(expectedFolder, value);
    }
    
    [Fact]
    public void Should_CreateFolder_WhenNotExists()
    {
        // arrange
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        runtime.IsOSPlatform(OSPlatform.Linux).Returns(true);
        var cmd = Substitute.For<ICommandProvider>();
        var fm = Substitute.For<IFileSystemManager>();
        var log = Substitute.For<ILogger<RunnerManager>>();
        var sut = new RunnerManager(runtime, cmd, fm, log);
        const string name = "test-runner";
        const string runnerFolder = $"/var/lib/{Constants.SafeAppName}/runners/{name}";
        fm.DirectoryExists(runnerFolder).Returns(false);
        
        // act
        _ = sut.GetRunnerFolder(name);

        // assert
        fm.Received().DirectoryCreate(runnerFolder);
    }
    
    [Fact]
    public void Should_NotCreateFolder_WhenExists()
    {
        // arrange
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        runtime.IsOSPlatform(OSPlatform.Linux).Returns(true);
        var cmd = Substitute.For<ICommandProvider>();
        var fm = Substitute.For<IFileSystemManager>();
        var log = Substitute.For<ILogger<RunnerManager>>();
        var sut = new RunnerManager(runtime, cmd, fm, log);
        const string name = "test-runner";
        const string runnerFolder = $"/var/lib/{Constants.SafeAppName}/runners/{name}";
        fm.DirectoryExists(runnerFolder).Returns(true);
        
        // act
        _ = sut.GetRunnerFolder(name);

        // assert
        fm.DidNotReceive().DirectoryCreate(runnerFolder);
    }
}
