using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Runners.Services;

public interface IRuntimeInformationProvider
{
    bool IsDebug { get; }
    Architecture ProcessArchitecture { get; }
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Keep it consistent as OSPlatform")]
    bool IsOSPlatform(OSPlatform osPlatform);
}

public sealed class RuntimeInformationProvider : IRuntimeInformationProvider
{
    public bool IsDebug =>
#if DEBUG
        true
#else
        false
#endif
        ;
    public Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;
    public bool IsOSPlatform(OSPlatform osPlatform) => RuntimeInformation.IsOSPlatform(osPlatform);
}