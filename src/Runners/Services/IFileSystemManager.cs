using Microsoft.Extensions.Logging;

namespace Runners.Services;

public interface IFileSystemManager
{
    Stream? GetFile(string path, FileMode fileMode, FileAccess fileAccess);
    bool DirectoryExists(string path);
    void DirectoryCreate(string path);
    void DirectoryDelete(string path, bool recursive);
    
    void DirectoryMove(string path, string destination);
    void DirectoryCopy(string path, string destination);
}

public sealed class FileSystemManager : IFileSystemManager
{
    private readonly ILogger<FileSystemManager> _logger;
    
    public FileSystemManager(ILogger<FileSystemManager> logger)
    {
        _logger = logger;
    }
    
    public Stream? GetFile(string path, FileMode fileMode, FileAccess fileAccess)
    {
        if (!File.Exists(path))
        {
            _logger.LogWarning("Getting file {File} using {FileMode} with {FileAccess} but not found.", path, fileMode, fileAccess);
            return null;
        }
        
        _logger.LogDebug("Getting file {File} using {FileMode} with {FileAccess}", path, fileMode, fileAccess);
        
        return File.Open(path, fileMode, fileAccess);
    }
    
    public bool DirectoryExists(string path)
    {
        _logger.LogDebug("Checking if directory {Path}", path);
        
        return Directory.Exists(path);
    }
    
    public void DirectoryCreate(string path)
    {
        _logger.LogDebug("Creating directory {Path}", path);
        
        Directory.CreateDirectory(path);
    }
    
    public void DirectoryDelete(string path, bool recursive)
    {
        _logger.LogDebug("Deleting directory {Path} (Recursive: {Recursive})", path, recursive);
        
        Directory.Delete(path, recursive);
    }

    public void DirectoryMove(string path, string destination)
    {
        _logger.LogDebug("Moving directory {Path} to {Destination}", path, destination);
        Directory.Move(path, destination);
    }
    
    public void DirectoryCopy(string path, string destination)
    {
        _logger.LogDebug("Copying directory {Path} to {Destination}", path, destination);
        
        var diSource = new DirectoryInfo(path);
        var diTarget = new DirectoryInfo(destination);

        CopyAll(diSource, diTarget);
    }

    private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        Directory.CreateDirectory(target.FullName);

        foreach (var fi in source.GetFiles())
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);

        foreach (var diSourceSubDir in source.GetDirectories())
        {
            var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }
}