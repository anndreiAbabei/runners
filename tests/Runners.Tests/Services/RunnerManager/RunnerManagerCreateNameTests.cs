using CSharpFunctionalExtensions;
using Runners.Services;

namespace Runners.Tests.Services;

public sealed class RunnerManagerCreateNameTests
{
    [Fact]
    public void Should_ReturnSuccessWithName_WhenLinkIsCorrect()
    {
        // arrange
        const string url = "https://github.com/UltraSlime/SlimeTools.Commander";
        const string expectedName = "UltraSlime_SlimeTools.Commander";
        var sut = new RunnerManager(null!, null!, null!, null!);

        // act
        var (isSuccess, _, value) = sut.CreateName(url);

        // assert
        Assert.True(isSuccess);
        Assert.Equal(expectedName, value);
    }
    
    [Fact]
    public void Should_ReturnFailure_WhenLinkIsNotCorrect()
    {
        // arrange
        const string url = "https://github.com/UltraSlime/SlimeTools.Commander/docs";
        const string expectedError = "Invalid format of GitHub url";
        var sut = new RunnerManager(null!, null!, null!, null!);

        // act
        var (isSuccess, _, _, error) = sut.CreateName(url);

        // assert
        Assert.False(isSuccess);
        Assert.Equal(expectedError, error);
    }
}
