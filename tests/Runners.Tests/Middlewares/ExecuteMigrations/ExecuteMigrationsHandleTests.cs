using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Runners.Middlewares;
using Runners.Persistence;
using SlimeTools.Commander;

namespace Runners.Tests.Middlewares;

public sealed class ExecuteMigrationsHandleTests
{
    [Fact]
    public async Task Should_ExecuteMigrations()
    {
        // Arrange
        var migrator = Substitute.For<IMigrator>();
        migrator.MigrateAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

        await using var dbContext = CreateDbContext(migrator);
        var middleware = new ExecuteMigrations(dbContext);
        var context = Substitute.For<ICommandContext>();
        MiddlewareHandler next = (_, _) => ValueTask.FromResult(Result.Success());

        // Act
        await middleware.Handle(context, next, CancellationToken.None);

        // Assert
        await migrator.Received(1).MigrateAsync(null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_InvokeNextMiddleware()
    {
        // Arrange
        var migrator = Substitute.For<IMigrator>();
        migrator.MigrateAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

        await using var dbContext = CreateDbContext(migrator);
        var middleware = new ExecuteMigrations(dbContext);
        var context = Substitute.For<ICommandContext>();

        var nextCalled = false;
        MiddlewareHandler next = (_, _) =>
        {
            nextCalled = true;
            return Result.Success().WrapToValueTask();
        };

        // Act
        await middleware.Handle(context, next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
    }

    private static TestDbContext CreateDbContext(IMigrator migrator)
    {
        var services = new ServiceCollection();
        services.AddEntityFrameworkSqlite();
        services.AddSingleton(migrator);

        var serviceProvider = services.BuildServiceProvider();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("DataSource=:memory:")
            .UseInternalServiceProvider(serviceProvider)
            .Options;

        return new TestDbContext(options);
    }

    private sealed class TestDbContext : DbContext, IRunnersDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<RunnerItem> RunnerItems { get; set; } = null!;
    }
}
