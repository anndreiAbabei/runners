using NSubstitute;
using Runners.Persistence;
using Runners.Services;

namespace Runners.Tests.Persistence;

public sealed class RunnersDbContextOnConfiguringTests
{
    [Fact]
    public void Should_UseSqlLiteConnection()
    {
        // arrange
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        const string expectedProvider = "Microsoft.EntityFrameworkCore.Sqlite";
        
        // act
        using var ctx = new RunnersDbContext(runtime);

        // assert
        Assert.Equal(expectedProvider, ctx.Database.ProviderName);
    }
}
