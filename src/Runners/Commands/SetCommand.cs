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
                      .Build();
    }

    public async ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        var runner = await _dbContext.RunnerItems.FirstOrDefaultAsync(d => d.Id == Data.RunnerId && !d.Deleted, cancellationToken);

        if (runner == null)
            return Failure("Runner not found.");

        var result = await _manager.SetRunner(runner, Data.State == RunnerSetState.Start, cancellationToken);
        
        _logger.LogInformation("Runner {RunnerId} is now in state {State}", runner.Id, Data.State);

        return result;
    }
    
    public sealed class SetCommandData : ICommandData
    {
        public required int RunnerId { get; init; }
        public required RunnerSetState State { get; init; }
    }
    
    public enum RunnerSetState
    {
        [CommandEnumValue("start")]
        Start,
        [CommandEnumValue("stop")]
        Stop
    }
}

