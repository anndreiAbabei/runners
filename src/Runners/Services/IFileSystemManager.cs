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
    public Stream? GetFile(string path, FileMode fileMode, FileAccess fileAccess)
    {
        return File.Exists(path) 
                   ? File.Open(path, fileMode, fileAccess) 
                   : null;
    }
    public bool DirectoryExists(string path) => Directory.Exists(path);
    public void DirectoryCreate(string path) => Directory.CreateDirectory(path);
    public void DirectoryDelete(string path, bool recursive) => Directory.Delete(path, recursive);
    
    public void DirectoryMove(string path, string destination) => Directory.Move(path, destination);
    public void DirectoryCopy(string path, string destination)
    {
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