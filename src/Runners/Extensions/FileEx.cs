namespace Runners.Extensions;

public static class FileEx
{
    public static string SetShellFileExtension(this string file)
    {
        var extension = Os.IsWindows ? "cmd" : "sh";
        
        return Path.ChangeExtension(file, extension);
    }
}
