namespace Runners.Persistence;

public sealed class RunnerItem
{
    public int Id { get; init; }
    
    public required string Name { get; set; }
    
    public required string GitUrl { get; set; }
    
    public required string FolderPath { get; set; }
    
    public required DateTimeOffset CreatedAt { get; set; }
    
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
    Deleted = 99
}
