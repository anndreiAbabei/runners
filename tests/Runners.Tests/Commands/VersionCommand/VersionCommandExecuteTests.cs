using NSubstitute;
using Runners.Commands;
using Runners.Services;
using SlimeTools.Commander;

namespace Runners.Tests.Commands;

public sealed class VersionCommandExecuteTests
{
    [Fact]
    public async Task Should_ReturnSuccessResult()
    {
        // arrange
        var print = Substitute.For<IConsolePrint>();
        var version = Substitute.For<IVersionProvider>();
        var ctx = Substitute.For<ICommandContext>();
        var sut = new VersionCommand(print, version);
        version.GetAppVersion().Returns(new Version(1, 2, 3));

        // act
        var result = await sut.Execute(ctx, CancellationToken.None);

        // assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Should_PrintAssemblyVersion()
    {
        // arrange
        var print = Substitute.For<IConsolePrint>();
        var version = Substitute.For<IVersionProvider>();
        var ctx = Substitute.For<ICommandContext>();
        var sut = new VersionCommand(print, version);
        var expectedVersion = new Version(1, 2, 3, 4);
        var expectedVersionStr = expectedVersion.ToString(3);
        version.GetAppVersion().Returns(expectedVersion);

        // act
        _ = await sut.Execute(ctx, CancellationToken.None);

        // assert
        print.Received().WriteLine(expectedVersionStr);
    }
}
