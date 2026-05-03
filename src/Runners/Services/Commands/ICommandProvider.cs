using Runners.Extensions;

namespace Runners.Services.Commands;

public interface ICommandProvider
{
    ICommand Shell();
    ICommand Create(string command);
}

public sealed class CommandProvider : ICommandProvider
{
    private readonly IRuntimeInformationProvider _runtimeProvider;
    
    public CommandProvider(IRuntimeInformationProvider runtimeProvider)
    {
        _runtimeProvider = runtimeProvider;

    }
    
    public ICommand Create(string command) => new CliCommand(command);
    
    public ICommand Shell()
    {
        var cmd = _runtimeProvider.IsWindows
                      ? "cmd"
                      : "bash";

        return Create(cmd);
    }
}