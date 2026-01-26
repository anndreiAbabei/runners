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
        
        public string GetShellExtension() => provider.IsWindows ? "cmd" : "sh";
    }
}
