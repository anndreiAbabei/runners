using System.Diagnostics.CodeAnalysis;

namespace Runners.Persistence;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "Configured in OnModelCreating")]
public sealed class RunnerItem
{
    public int Id { get; init; }
    
    public required string Name { get; init; }
    
    public required string GitUrl { get; init; }
    
    public required string FolderPath { get; init; }
    
    public required DateTimeOffset CreatedAt { get; init; }
    
    public RunnerState State { get; set; }
    
    public string? Tag { get; set; }
    
    public bool Deleted { get; set; }
}

public enum RunnerState
{
    Added = 0,
    Downloaded = 1,
    Configured = 2,
    Installed = 3,
    Started = 4,
    Stopped = 5,
    Deleted = 99,
}
