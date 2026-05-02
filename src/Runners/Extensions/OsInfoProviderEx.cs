using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Runners.Services;

namespace Runners.Extensions;

public static class OsInfoProviderEx
{
    extension(IRuntimeInformationProvider provider)
    {
        public bool IsWindows => provider.IsOSPlatform(OSPlatform.Windows);
        public bool IsMacOs => provider.IsOSPlatform(OSPlatform.OSX);
        public bool IsLinux => provider.IsOSPlatform(OSPlatform.Linux);
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Keep it consistent")]
        public bool IsFreeBSD => provider.IsOSPlatform(OSPlatform.FreeBSD);
        
        public string GetShellExtension() => provider.IsWindows ? "cmd" : "sh";
    }
}

public static class FileLocationProviderEx
{
    extension(IRuntimeInformationProvider provider)
    {
        public string GetStateDir(IFileSystemManager fileSystem)
        {
            string folder;
            
            if (provider.IsDebug)
                folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else if (provider.IsLinux)
                folder = "/var/lib";
            else if (provider.IsFreeBSD)
                folder = "/var/db";
            else if (provider.IsMacOs)
                folder = "/Library/Application Support";
            else
                folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            
            folder = Path.Combine(folder, Constants.SafeAppName);
            
            fileSystem?.DirectoryCreate(folder);

            return folder;
        }
    }
}
