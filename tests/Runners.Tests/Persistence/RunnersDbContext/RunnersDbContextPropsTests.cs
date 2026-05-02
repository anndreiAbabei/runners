using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        
        await using var ctx = new RunnersDbContext(runtime, options, fs, appSettingsManager, log);
        await ctx.Database.MigrateAsync();
        
        // act
        var items = await ctx.RunnerItems.ToListAsync();

        // assert
        Assert.Empty(items);
    }
}
