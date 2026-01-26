using CliWrap;
using Runners.Extensions;

namespace Runners.Services;

public interface ICommandProvider
{
    Command Shell();
    Command Create(string command);
}

public sealed class CommandProvider : ICommandProvider
{
    private readonly IRuntimeInformationProvider _runtimeProvider;
    
    public CommandProvider(IRuntimeInformationProvider runtimeProvider)
    {
        _runtimeProvider = runtimeProvider;

    }
    
    public Command Create(string command) => Cli.Wrap(command);
    
    public Command Shell()
    {
        var cmd = _runtimeProvider.IsWindows
                      ? "cmd"
                      : "sh";

        return Create(cmd);
    }
}