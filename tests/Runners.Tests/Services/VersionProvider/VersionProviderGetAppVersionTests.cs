using System.Reflection;
using Runners.Services;

namespace Runners.Tests.Services;

public sealed class VersionProviderGetAppVersionTests
{
    [Fact]
    public void Should_ReturnAssemblyVersion()
    {
        // arrange
        var expectedVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 0);
        var sut = new VersionProvider();

        // act
        var result = sut.GetAppVersion();

        // assert
        Assert.Equal(expectedVersion, result);
    }
}
