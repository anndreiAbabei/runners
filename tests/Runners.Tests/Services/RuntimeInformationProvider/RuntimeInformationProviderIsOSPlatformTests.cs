using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Runners.Services;

namespace Runners.Tests.Services;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Keep it consistent as OSPlatform")]
public sealed class RuntimeInformationProviderIsOSPlatformTests
{
    [Fact]
    public void Should_ReturnExpectedPlatformResult()
    {
        // arrange
        var expectedResult = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var sut = new RuntimeInformationProvider();

        // act
        var result = sut.IsOSPlatform(OSPlatform.Windows);

        // assert
        Assert.Equal(expectedResult, result);
    }
}
