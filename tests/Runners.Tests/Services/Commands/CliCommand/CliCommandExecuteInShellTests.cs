using Runners.Services.Commands;

namespace Runners.Tests.Services.Commands;

public sealed class CliCommandExecuteInShellTests
{
    [Fact]
    public async Task Should_ReturnSuccess_WhenCommandSucceeds()
    {
        // arrange
        var sut = CreateShellCommand(SuccessScript());

        // act
        var result = await sut.ExecuteInShell(CancellationToken.None);

        // assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCommandFails()
    {
        // arrange
        var sut = CreateShellCommand(FailureScript());

        // act
        var result = await sut.ExecuteInShell(CancellationToken.None);

        // assert
        Assert.True(result.IsFailure);
    }

    private static CliCommand CreateShellCommand(string script)
    {
        return OperatingSystem.IsWindows()
            ? new CliCommand("cmd", ["/c", script])
            : new CliCommand("sh", ["-c", script]);
    }

    private static string SuccessScript()
    {
        return OperatingSystem.IsWindows()
            ? "exit /b 0"
            : "exit 0";
    }

    private static string FailureScript()
    {
        return OperatingSystem.IsWindows()
            ? "exit /b 1"
            : "exit 1";
    }
}
