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
    private IAppSettings? _appSettings;
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
        if(_appSettings != null)
            return _appSettings;
        
        await using var file = _fileSystemManager.GetFile(AppSettingsFileName, FileMode.Open, FileAccess.Read);

        if (file == null)
            return _appSettings = new AppSettings();
        
        _appSettings = await JsonSerializer.DeserializeAsync<AppSettings>(file, Options, cancellationToken) ?? new AppSettings();

        return _appSettings;
    }

    public async ValueTask Write(IAppSettings appSettings, CancellationToken cancellationToken = default)
    {
        await using var file = _fileSystemManager.GetFile(AppSettingsFileName, FileMode.OpenOrCreate, FileAccess.Write);
        
        if(file == null)
            return;
        
        _appSettings = appSettings;
        
        await JsonSerializer.SerializeAsync(file, _appSettings, Options, cancellationToken);
    }
}