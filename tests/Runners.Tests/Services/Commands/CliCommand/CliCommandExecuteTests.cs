using Runners.Services.Commands;

namespace Runners.Tests.Services.Commands;

public sealed class CliCommandExecuteTests
{
    [Fact]
    public async Task Should_ReturnOutput_WhenCommandSucceeds()
    {
        // arrange
        const string expectedOutput = "hello";
        var sut = CreateShellCommand(OutputScript(expectedOutput));

        // act
        var result = await sut.Execute(CancellationToken.None);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedOutput, result.Value.TrimEnd());
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCommandFails()
    {
        // arrange
        const string expectedError = "error";
        var sut = CreateShellCommand(FailureScript(expectedError));

        // act
        var result = await sut.Execute(CancellationToken.None);

        // assert
        Assert.True(result.IsFailure);
        Assert.Contains(expectedError, result.Error);
    }

    private static CliCommand CreateShellCommand(string script)
    {
        return OperatingSystem.IsWindows()
            ? new CliCommand("cmd", ["/c", script])
            : new CliCommand("sh", ["-c", script]);
    }

    private static string OutputScript(string echo) => $"echo {echo}";

    private static string FailureScript(string echo) => OperatingSystem.IsWindows()
                                                            ? $"echo {echo} 1>&2 & exit /b 1"
                                                            : $"echo {echo} 1>&2; exit 1";
}
