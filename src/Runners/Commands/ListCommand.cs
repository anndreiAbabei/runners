using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Runners.Persistence;
using Runners.Services;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed class ListCommand : BaseCommand<ListCommand.ListCommandData>, ICommand<ListCommand.ListCommandData>
{
    private readonly IRunnersDbContext _dbContext;
    private readonly IRunnerManager _manager;
    private readonly IConsolePrint _console;
    
    public ListCommand(IRunnersDbContext dbContext, IRunnerManager manager, IConsolePrint console)
    {
        _dbContext = dbContext;
        _manager = manager;
        _console = console;
    }
    
    public static ICommandStructure CreateCommand(ICommandBuilder builder)
    {
        return builder.Create<ListCommandData>("list", new CommandDetails { Description = "List all runners configured." })
                      .WithFlag(d => d.IncludeSvcStatus, "include-svc", new FlagOptions{Description = "Include service status in the check (default: false)", Optional = true})
                      .Build();
    }

    public async ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        var runners = await _dbContext.RunnerItems.Where(d => !d.Deleted).ToListAsync(cancellationToken);

        if (runners.Count <= 0)
            return Failure("No runners configured.");

        var sb = new StringBuilder();
        
        foreach (var runner in runners)
        {
            sb.Append(runner.Id).Append(" - ").Append(runner.Name)
              .Append(": ").Append(runner.State);

            if (Data.IncludeSvcStatus)
            {
                var (_, isFailure, value, error) = await _manager.GetState(runner, cancellationToken);

                if (isFailure)
                    return Failure($"Runner {runner.Id} ({runner.Name}) could not be checked due to error: {error}.");

                sb.Append(" (").Append(value).Append(')');
            }

            if (!string.IsNullOrEmpty(runner.Tag))
                sb.Append(" [").Append(runner.Tag).Append(']');

            sb.AppendLine();
        }
        
        _console.WriteLine(sb.ToString());

        return Success;
    }
    
    public sealed class ListCommandData : ICommandData
    {
        public bool IncludeSvcStatus { get; init; }
    }
}
