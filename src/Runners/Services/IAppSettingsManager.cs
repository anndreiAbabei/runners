using System.Text.Json;
using Runners.Persistence;

namespace Runners.Services;

public interface IAppSettingsManager
{
    ValueTask<IAppSettings> Read(CancellationToken cancellationToken = default);
    ValueTask Write(IAppSettings appSettings, CancellationToken cancellationToken = default);
}

public sealed class AppSettingsManager : IAppSettingsManager
{
    private readonly IFileSystemManager _fileSystemManager;
    private static readonly JsonSerializerOptions? Options = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    
    internal const string AppSettingsFileName = "appsettings.json";

    public AppSettingsManager(IFileSystemManager fileSystemManager)
    {
        _fileSystemManager = fileSystemManager;
    }
    
    public async ValueTask<IAppSettings> Read(CancellationToken cancellationToken = default)
    {
        await using var file = _fileSystemManager.GetFile(AppSettingsFileName, FileMode.Open, FileAccess.Read);

        if (file == null)
            return new AppSettings();
        
        var settings = await JsonSerializer.DeserializeAsync<AppSettings>(file, Options, cancellationToken);
        
        return settings ?? new AppSettings();
    }

    public async ValueTask Write(IAppSettings appSettings, CancellationToken cancellationToken = default)
    {
        await using var file = _fileSystemManager.GetFile(AppSettingsFileName, FileMode.OpenOrCreate, FileAccess.Write);
        
        if(file == null)
            return;
        
        await JsonSerializer.SerializeAsync(file, appSettings, Options, cancellationToken);
    }
}