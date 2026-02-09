using Runners.Services.Commands;

namespace Runners.Tests.Services.Commands;

public sealed class CliCommandWithWorkingDirectoryTests
{
    [Fact]
    public void Should_SetWorkingDirectory()
    {
        // arrange
        const string expectedWorkingDirectory = "/tmp";
        var sut = new CliCommand("test");

        // act
        sut.WithWorkingDirectory(expectedWorkingDirectory);

        // assert
        Assert.Equal(expectedWorkingDirectory, sut.WorkingDirectory);
    }
}
