using System.Reflection;

namespace Runners.Services;

public interface IVersionProvider
{
    Version GetAppVersion();
}

public sealed class VersionProvider : IVersionProvider
{
    public Version GetAppVersion() => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0);
}