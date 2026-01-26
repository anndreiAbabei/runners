using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Runners.Persistence;
using Runners.Services;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed class DeleteCommand : BaseCommand<DeleteCommand.DeleteCommandData>, ICommand<DeleteCommand.DeleteCommandData>
{
    private readonly IRunnersDbContext _dbContext;
    private readonly IRunnerManager _manager;
    private readonly ILogger<SetCommand> _logger;

    public DeleteCommand(IRunnersDbContext dbContext, 
                         IRunnerManager manager, 
                         ILogger<SetCommand> logger)
    {
        _dbContext = dbContext;
        _manager = manager;
        _logger = logger;
    }
    
    public static ICommandStructure CreateCommand(ICommandBuilder builder)
    {
        return builder.Create<DeleteCommandData>("delete", new CommandDetails { Description = "Change the state of a runner." })
                      .WithArg(d => d.RunnerId, new ArgumentOptions { Description = "Id of the runner that you want to set the state.", Optional = false })
                      .WithFlag(d => d.Understand, "yes", new FlagOptions{Description = "Required to set delete a runner."})
                      .Build();
    }

    public async ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        if (!Data.Understand)
            return Failure("To delete a runner you must invoke the delete command with the '--yes' flag." +
                           " Doing that you understand that the action is permanent and the action cannot be undone.");
        
        var runner = await _dbContext.RunnerItems.FindAsync([Data.RunnerId], cancellationToken);

        if (runner == null)
            return Failure("Runner not found.");

        var result = await _manager.DeleteRunner(runner, cancellationToken);

        runner.Deleted = true;
        runner.State = RunnerState.Deleted;
        _dbContext.RunnerItems.Update(runner);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Runner {RunnerId} is now in deleted.", runner.Id);

        return result;
    }
    
    public sealed class DeleteCommandData : ICommandData
    {
        public required int RunnerId { get; init; }
        public bool Understand { get; init; }
    }
}

