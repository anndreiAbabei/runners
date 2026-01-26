namespace Runners.Services;

public interface IFileSystemManager
{
    bool DirectoryExists(string path);
    void DirectoryCreate(string path);
    void DirectoryDelete(string path, bool recursive);
}

public sealed class FileSystemManager : IFileSystemManager
{
    public bool DirectoryExists(string path) => Directory.Exists(path);
    public void DirectoryCreate(string path) => Directory.CreateDirectory(path);
    public void DirectoryDelete(string path, bool recursive) => Directory.Delete(path, recursive);
}