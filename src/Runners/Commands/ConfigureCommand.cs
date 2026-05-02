using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Runners.Persistence;
using Runners.Services;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed partial class ConfigureCommand : BaseCommand<ConfigureCommand.ConfigureCommandData>, ICommand<ConfigureCommand.ConfigureCommandData>
{
    private readonly IRunnersDbContext _dbContext;
    private readonly IRunnerManager _manager;
    private readonly IAppSettingsManager _appSettingsManager;
    private readonly IFileSystemManager _fileSystemManager;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ConfigureCommand> _logger;

    public ConfigureCommand(IRunnersDbContext dbContext, 
                            IRunnerManager manager, 
                            IAppSettingsManager appSettingsManager,
                            IFileSystemManager fileSystemManager,
                            TimeProvider timeProvider,
                            ILogger<ConfigureCommand> logger)
    {
        _dbContext = dbContext;
        _manager = manager;
        _appSettingsManager = appSettingsManager;
        _fileSystemManager = fileSystemManager;
        _timeProvider = timeProvider;
        _logger = logger;
    }
    
    public static ICommandStructure CreateCommand(ICommandBuilder builder)
    {
        return builder.Create<ConfigureCommandData>("config")
                      .WithFlag(d => d.RunnersFolder, options: FlagOptions.Description("Set the runners folder. " +
                                                                                       "If there are existing runners in the previous/default runners folder you must provide " +
                                                                                       $"the {nameof(ConfigureCommandData.MigrateRunners)} flag.")
                                                                          .Optional()
                                                                          .Named("runners-folder")
                                                                          .Example("./runners"))
                      .WithFlag(d => d.MigrateRunners, options: FlagOptions.Description($"Strategy used when the {nameof(ConfigureCommandData.RunnersFolder)} is used to migrate the existing runners." +
                                                                                        $"- ignore: Ignores the existing runners folder." +
                                                                                        $"- copy: If any existing runners, copy the them into the new location." +
                                                                                        $"- move: If any existing runners, move the them into the new location." +
                                                                                        $"- clean: If any existing runners, delete them! (without bringing it to the new location) - WARNING: IT DELETES DATA!")
                                                                           .Optional()
                                                                           .Named("migrate-runners")
                                                                           .Example("./runners"))
                      .Build();
    }
    
    public async ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(Data.RunnersFolder))
        {
            var result = await MoveChangeRunnersFolder(Data.RunnersFolder, context, cancellationToken);
            if (result.IsFailure)
                return result;
        }

        return Success;
    }
    
    public class ConfigureCommandData : ICommandData, IAppSettings
    {
        public string? RunnersFolder { get; set; }
        public MigrateRunnersStrategy? MigrateRunners { get; init; }
    }
}

