namespace Runners.Persistence;

public interface IAppSettings
{
    string? RunnersFolder { get; set; }
    string? DbFilePath { get; set; }
}

public sealed class AppSettings : IAppSettings
{
    public string? RunnersFolder { get; set; }
    public string? DbFilePath { get; set; }
}