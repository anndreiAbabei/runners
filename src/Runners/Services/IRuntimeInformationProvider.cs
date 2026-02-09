using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Runners.Services;

public interface IRuntimeInformationProvider
{
    Architecture ProcessArchitecture { get; }
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Keep it consistent as OSPlatform")]
    bool IsOSPlatform(OSPlatform osPlatform);
}

public sealed class RuntimeInformationProvider : IRuntimeInformationProvider
{
    public Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;
    public bool IsOSPlatform(OSPlatform osPlatform) => RuntimeInformation.IsOSPlatform(osPlatform);
}