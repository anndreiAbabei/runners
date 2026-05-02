using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Runners.Persistence;
using Runners.Services;
using Runners.Services.Commands;

namespace Runners.Tests.Services;

[SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "Used in mocks")]
public sealed class RunnerManagerGetRunnerFolderTests
{
    [Fact]
    public async Task Should_ReturnSuccess_WithFolderName()
    {
        // arrange
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        runtime.IsOSPlatform(OSPlatform.Linux).Returns(true);
        var cmd = Substitute.For<ICommandProvider>();
        var fm = Substitute.For<IFileSystemManager>();
        var settings = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnerManager>>();
        var sut = new RunnerManager(runtime, cmd, fm, settings, log);
        const string name = "test-runner";
        const string expectedFolder = $"/var/lib/{Constants.SafeAppName}/runners/{name}";
        
        settings.Read(Arg.Any<CancellationToken>())
                .Returns(_ => ValueTask.FromResult(Substitute.For<IAppSettings>()));
        
        // act
        var (isSuccess, _, value) = await sut.GetRunnerFolder(name, CancellationToken.None);

        // assert
        Assert.True(isSuccess);
        Assert.Equal(expectedFolder, value);
    }
    
    [Fact]
    public async Task Should_CreateFolder_WhenNotExists()
    {
        // arrange
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        runtime.IsOSPlatform(OSPlatform.Linux).Returns(true);
        var cmd = Substitute.For<ICommandProvider>();
        var fm = Substitute.For<IFileSystemManager>();
        var settings = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnerManager>>();
        var sut = new RunnerManager(runtime, cmd, fm, settings, log);
        const string name = "test-runner";
        const string runnerFolder = $"/var/lib/{Constants.SafeAppName}/runners/{name}";
        fm.DirectoryExists(runnerFolder).Returns(false);
        
        settings.Read(Arg.Any<CancellationToken>())
                .Returns(_ => ValueTask.FromResult(Substitute.For<IAppSettings>()));
        
        // act
        _ = await sut.GetRunnerFolder(name, CancellationToken.None);

        // assert
        fm.Received().DirectoryCreate(runnerFolder);
    }
    
    [Fact]
    public async Task Should_NotCreateFolder_WhenExists()
    {
        // arrange
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        runtime.IsOSPlatform(OSPlatform.Linux).Returns(true);
        var cmd = Substitute.For<ICommandProvider>();
        var fm = Substitute.For<IFileSystemManager>();
        var settings = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnerManager>>();
        var sut = new RunnerManager(runtime, cmd, fm, settings, log);
        const string name = "test-runner";
        const string runnerFolder = $"/var/lib/{Constants.SafeAppName}/runners/{name}";
        fm.DirectoryExists(runnerFolder).Returns(true);
        
        settings.Read(Arg.Any<CancellationToken>())
                .Returns(_ => ValueTask.FromResult(Substitute.For<IAppSettings>()));
        
        // act
        _ = await sut.GetRunnerFolder(name, CancellationToken.None);

        // assert
        fm.DidNotReceive().DirectoryCreate(runnerFolder);
    }
}
