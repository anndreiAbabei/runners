using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Runners.Persistence;
using Runners.Services;

namespace Runners.Tests.Persistence;

public sealed class RunnersDbContextPropsTests
{
    [Fact]
    public async Task Should_SetProperties()
    {
        // arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<RunnersDbContext>()
                      .UseSqlite(connection)
                      .Options;
        var runtime = Substitute.For<IRuntimeInformationProvider>();

        await using var ctx = new RunnersDbContext(runtime, options);
        await ctx.Database.MigrateAsync();
        
        // act
        var items = await ctx.RunnerItems.ToListAsync();

        // assert
        Assert.Empty(items);
    }
}
