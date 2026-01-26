using System.Reflection;
using CSharpFunctionalExtensions;
using Runners.Services;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed class VersionCommand : BaseCommand, ICommand
{
    private readonly IConsolePrint _consolePrint;
    private readonly IVersionProvider _versionProvider;

    public VersionCommand(IConsolePrint consolePrint, IVersionProvider versionProvider)
    {
        _consolePrint = consolePrint;
        _versionProvider = versionProvider;
    }

    public static ICommandStructure CreateCommand(ICommandBuilder builder)
    {
        return builder.Create("version", new CommandDetails { Description = "Prints out the version of this application." })
                      .Build();
    }

    public ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        _consolePrint.WriteLine(_versionProvider.GetAppVersion().ToString(3));

        return SuccessTask;
    }
}
