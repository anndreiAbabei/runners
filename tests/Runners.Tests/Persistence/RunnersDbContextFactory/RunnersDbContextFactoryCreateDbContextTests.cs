using Runners.Persistence;

namespace Runners.Tests.Persistence;

public sealed class RunnersDbContextFactoryCreateDbContextTests
{
    [Fact]
    public void Should_CreateNewDbContext()
    {
        // arrange
        var sut = new RunnersDbContextFactory();
        
        // act
        var result = sut.CreateDbContext([]);

        // assert
        Assert.NotNull(result);
    }
}
