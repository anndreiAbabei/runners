using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Runners.Persistence;
using Runners.Services;

namespace Runners.Tests.Persistence;

[SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "Used for mocking")]
public sealed class RunnersDbContextOnConfiguringTests
{
    [Fact]
    public void Should_UseSqlLiteConnection()
    {
        // arrange
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        const string expectedProvider = "Microsoft.EntityFrameworkCore.Sqlite";
        
        // act
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);

        // assert
        Assert.Equal(expectedProvider, ctx.Database.ProviderName);
    }
}
