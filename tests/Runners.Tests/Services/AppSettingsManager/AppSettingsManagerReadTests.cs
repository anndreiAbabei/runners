using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using NSubstitute;
using Runners.Persistence;
using Runners.Services;

namespace Runners.Tests.Services;

public sealed class AppSettingsManagerReadTests
{
    [Fact]
    public void Should_AppSettingsFileCheck()
    {
        // arrange
        const string expectedFile = "appsettings.json";
        
        // act
        const string result = AppSettingsManager.AppSettingsFileName;

        // assert
        Assert.Equal(expectedFile, result);
    }
    
    [Fact]
    public async Task Should_ReturnEmptyNewAppSettings_IfFileDoesNotExist()
    {
        // arrange
        var fs = Substitute.For<IFileSystemManager>();
        fs.GetFile(AppSettingsManager.AppSettingsFileName, FileMode.Open, FileAccess.Read)
          .Returns((Stream?)null);
        var sut = new AppSettingsManager(fs);

        // act
        var result = await sut.Read();

        // assert
        Assert.IsType<AppSettings>(result);
    }

    [Fact]
    public async Task Should_DeserializeAppSettingsFile()
    {
        // arrange
        var expectedAppSettings = new AppSettings
        {
            RunnersFolder = "test"
        };
        using var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, expectedAppSettings);
        ms.Seek(0, SeekOrigin.Begin);
        var fs = Substitute.For<IFileSystemManager>();
        fs.GetFile(AppSettingsManager.AppSettingsFileName, FileMode.Open, FileAccess.Read)
          .Returns(ms);
        var sut = new AppSettingsManager(fs);

        // act
        var result = await sut.Read();

        // assert
        Assert.Equal(expectedAppSettings, result, AppSettingsComparer.Default);
    }
}

[ExcludeFromCodeCoverage]
internal sealed class AppSettingsComparer : IEqualityComparer<IAppSettings>
{
    public static IEqualityComparer<IAppSettings> Default { get; } = new AppSettingsComparer();
        
    public bool Equals(IAppSettings? x, IAppSettings? y)
    {
        if (ReferenceEquals(x, y))
            return true;
            
        if (x is null)
            return false;
            
        if (y is null)
            return false;
            
        if (x.GetType() != y.GetType())
            return false;
            
        return x.RunnersFolder == y.RunnersFolder;
    }
        
    public int GetHashCode(IAppSettings obj) => obj.RunnersFolder != null ? obj.RunnersFolder.GetHashCode() : 0;
}
