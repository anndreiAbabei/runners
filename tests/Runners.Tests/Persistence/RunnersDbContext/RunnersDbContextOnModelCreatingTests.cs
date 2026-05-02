using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Runners.Persistence;
using Runners.Services;

namespace Runners.Tests.Persistence;

[SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "Used for mocking")]
public sealed class RunnersDbContextOnModelCreatingTests
{
    [Fact]
    public void Should_HaveIdAsKey()
    {
        // arrange
        const string expectedProperty = nameof(RunnerItem.Id);
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));
        
        // assert
        Assert.NotNull(entity);
        var pk = entity.FindPrimaryKey();
        Assert.NotNull(pk);
        var idProp = Assert.Single(pk.Properties);
        Assert.Equal(expectedProperty, idProp.Name);
    }
    
    [Fact]
    public void Should_IdBeAutoIncrement()
    {
        // arrange
        const string propertyName = nameof(RunnerItem.Id);
        const ValueGenerated expectedGeneration = ValueGenerated.OnAdd;
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));
        
        // assert
        Assert.NotNull(entity);
        var prop = entity.FindProperty(propertyName);
        Assert.NotNull(prop);
        Assert.Equal(expectedGeneration, prop.ValueGenerated);
    }

    [Fact]
    public void Should_HaveRequiredName_WithMaxLength()
    {
        // arrange
        const string expectedProperty = nameof(RunnerItem.Name);
        const int expectedLength = 100;
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));

        // assert
        AssertRequiredWithMaxLen(entity, expectedProperty, expectedLength);
    }

    [Fact]
    public void Should_HaveRequiredGitUrl_WithMaxLength()
    {
        // arrange
        const string expectedProperty = nameof(RunnerItem.GitUrl);
        const int expectedLength = 200;
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));

        // assert
        AssertRequiredWithMaxLen(entity, expectedProperty, expectedLength);
    }

    [Fact]
    public void Should_HaveRequiredFolderPath_WithMaxLength()
    {
        // arrange
        const string expectedProperty = nameof(RunnerItem.FolderPath);
        const int expectedLength = 200;
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));

        // assert
        AssertRequiredWithMaxLen(entity, expectedProperty, expectedLength);
    }

    [Fact]
    public void Should_HaveRequiredCreatedAt()
    {
        // arrange
        const string expectedProperty = nameof(RunnerItem.CreatedAt);
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));

        // assert
        Assert.NotNull(entity);
        var prop = entity.FindProperty(expectedProperty);
        
        Assert.NotNull(prop);
        Assert.False(prop.IsNullable);
    }

    [Fact]
    public void Should_HaveState_WithDefault()
    {
        // arrange
        const string expectedProperty = nameof(RunnerItem.State);
        const RunnerState expectedState = RunnerState.Added;
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));

        // assert
        Assert.NotNull(entity);
        var prop = entity.FindProperty(expectedProperty);
        
        Assert.NotNull(prop);
        Assert.Equal(expectedState, prop.GetDefaultValue());
    }

    [Fact]
    public void Should_HaveTag_WithMaxLength()
    {
        // arrange
        const string expectedProperty = nameof(RunnerItem.Tag);
        const int expectedLength = 200;
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));

        // assert
        AssertRequiredWithMaxLen(entity, expectedProperty, expectedLength, false);
    }

    [Fact]
    public void Should_HaveDeleted_WithDefault()
    {
        // arrange
        const string expectedProperty = nameof(RunnerItem.Deleted);
        const bool expectedDeleted = false;
        var runtime = Substitute.For<IRuntimeInformationProvider>();
        var fs = Substitute.For<IFileSystemManager>();
        var appSettingsManager = Substitute.For<IAppSettingsManager>();
        var log = Substitute.For<ILogger<RunnersDbContext>>();
        using var ctx = new RunnersDbContext(runtime, fs, appSettingsManager, log);
        
        appSettingsManager.Read(Arg.Any<CancellationToken>())
                          .Returns(_ => ValueTask.FromResult<IAppSettings>(new AppSettings()));
        
        // act
        var entity = ctx.Model.FindEntityType(typeof(RunnerItem));

        // assert
        Assert.NotNull(entity);
        var prop = entity.FindProperty(expectedProperty);
        
        Assert.NotNull(prop);
        Assert.Equal(expectedDeleted, prop.GetDefaultValue());
    }
    
    private static void AssertRequiredWithMaxLen(IEntityType? entity, string propName, int maxLen, bool required = true)
    {
        Assert.NotNull(entity);
        
        var prop = entity.FindProperty(propName);
        
        Assert.NotNull(prop);
        Assert.Equal(required, !prop.IsNullable);
        Assert.Equal(maxLen, prop.GetMaxLength());
    }
}
