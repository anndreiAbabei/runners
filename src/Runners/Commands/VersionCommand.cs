using System.Reflection;
using CSharpFunctionalExtensions;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed class VersionCommand : BaseCommand, ICommand
{
    private readonly IConsolePrint _consolePrint;
    
    public VersionCommand(IConsolePrint consolePrint)
    {
        _consolePrint = consolePrint;
    }

    public static ICommandStructure CreateCommand(ICommandBuilder builder)
    {
        return builder.Create("version")
                      .Build();
    }

    public ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        _consolePrint.WriteLine(Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "UNK");

        return SuccessTask;
    }
}
