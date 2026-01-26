using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Runners.Persistence;
using Runners.Services;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed class ListCommand : BaseCommand, ICommand
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
        return builder.Create("list", new CommandDetails { Description = "List all runners configured." })
                      .Build();
    }

    public async ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        var runners = await _dbContext.RunnerItems.Where(d => !d.Deleted).ToListAsync(cancellationToken);

        if (runners.Count <= 0)
            return Failure("No runners configured.");
        
        foreach (var runner in runners)
        {
            var (_, isFailure, value, error) = await _manager.GetState(runner, cancellationToken);

            if (isFailure)
                return Failure($"Runner {runner.Id} ({runner.Name}) could not be checked due to error: {error}.");
            
            _console.WriteLine($"{runner.Id} - {runner.Name}: {value}{(string.IsNullOrEmpty(runner.Tag) ? "" : $"[{runner.Tag}]")}");
        }

        return Success;
    }
}
