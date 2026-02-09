using System.Runtime.InteropServices;
using Runners.Services;

namespace Runners.Tests.Services;

public sealed class RuntimeInformationProviderProcessArchitectureTests
{
    [Fact]
    public void Should_ReturnProcessArchitecture()
    {
        // arrange
        var expectedArchitecture = RuntimeInformation.ProcessArchitecture;
        var sut = new RuntimeInformationProvider();

        // act
        var result = sut.ProcessArchitecture;

        // assert
        Assert.Equal(expectedArchitecture, result);
    }
}
