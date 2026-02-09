using Runners.Services;

namespace Runners.Tests.Services;

public sealed class RuntimeInformationProviderPropsTests
{
    [Fact]
    public void Should_ShouldBeConfiguredAsProfile()
    {
        // arrange
        const bool expectedValue =
#if DEBUG
        true
#else
        false
#endif
            ;
        var sut = new RuntimeInformationProvider();

        // act
        var result = sut.IsDebug;

        // assert
        Assert.Equal(expectedValue, result);
    }
}
