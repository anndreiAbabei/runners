using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Runners.Persistence;
using Runners.Services;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed class SetCommand : BaseCommand<SetCommand.SetCommandData>, ICommand<SetCommand.SetCommandData>
{
    private readonly IRunnersDbContext _dbContext;
    private readonly IRunnerManager _manager;
    private readonly ILogger<SetCommand> _logger;

    public SetCommand(IRunnersDbContext dbContext, 
                      IRunnerManager manager, 
                      ILogger<SetCommand> logger)
    {
        _dbContext = dbContext;
        _manager = manager;
        _logger = logger;
    }
    
    public static ICommandStructure CreateCommand(ICommandBuilder builder)
    {
        return builder.Create<SetCommandData>("set", new CommandDetails { Description = "Change the state of a runner." })
                      .WithArg(d => d.RunnerId, new ArgumentOptions { Description = "Id of the runner that you want to set the state.", Optional = false })
                      .WithArg(d => d.State, new ArgumentOptions { Description = "State of the runner.", Optional = false })
                      .WithFlag(d => d.Force, "force", new FlagOptions{ Description = "Force set of the runner, bypassing validation of current state.", Optional = false })
                      .Build();
    }

    public async ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        var runner = await _dbContext.RunnerItems.FirstOrDefaultAsync(d => d.Id == Data.RunnerId && !d.Deleted, cancellationToken);

        if (runner == null)
            return Failure("Runner not found.");

        if (runner.State < RunnerState.Installed && !Data.Force)
            return Failure("Runner was not completely installed. If you are sure that it is ok, try again with --force");

        var result = await _manager.SetRunner(runner, Data.State == RunnerSetState.Start, cancellationToken);

        if (result.IsFailure)
            return result;

        var newState = MapState(Data.State);

        if (runner.State != newState || Data.Force)
        {
            runner.State = newState;
            _dbContext.RunnerItems.Update(runner);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Runner {RunnerId} is now in state {State}", runner.Id, runner.State);

        return Result.Success();
    }
    private static RunnerState MapState(RunnerSetState state)
    {
        return state switch
        {
            RunnerSetState.Start => RunnerState.Started,
            RunnerSetState.Stop => RunnerState.Stopped,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    public sealed class SetCommandData : ICommandData
    {
        public required int RunnerId { get; init; }
        public required RunnerSetState State { get; init; }
        public bool Force { get; init; }
    }
    
    public enum RunnerSetState
    {
        [CommandEnumValue("start")]
        Start,
        [CommandEnumValue("stop")]
        Stop
    }
}

