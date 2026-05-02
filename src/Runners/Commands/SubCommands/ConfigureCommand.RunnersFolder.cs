using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Runners.Persistence;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed partial class ConfigureCommand
{
    private async ValueTask<Result> MoveChangeRunnersFolder(string newLocation, ICommandContext context, CancellationToken cancellationToken)
    {
        if(Data.MigrateRunners == null)
        {
            var runnersFolderFlag = Data.GetName(context, d => d.RunnersFolder, true);
            var migrateStrategyFlag = Data.GetName(context, d => d.MigrateRunners, true);

            return Failure($"Using the {runnersFolderFlag} requires the {migrateStrategyFlag} flag.");
        }
        var migrationStrategy = Data.MigrateRunners.Value;

        var existingRunners = await _dbContext.RunnerItems.Where(ri => !ri.Deleted)
                                              .ToListAsync(cancellationToken);
        
        if(existingRunners.Count > 0 && migrationStrategy != MigrateRunnersStrategy.Ignore)
        {
            var resultMigration = await MigrateRunners(existingRunners, newLocation, migrationStrategy, cancellationToken);
            if (resultMigration.IsFailure)
                return resultMigration;
        }

        var settings = await _appSettingsManager.Read(cancellationToken);
        
        settings.RunnersFolder = newLocation;
        
        await _appSettingsManager.Write(settings, cancellationToken);
        
        return Success;
    }
    
    private async ValueTask<Result> MigrateRunners(List<RunnerItem> existingRunners, string newLocation, MigrateRunnersStrategy migrationStrategy, CancellationToken cancellationToken)
    {
        foreach (var runnerItem in existingRunners)
        {
            var resultMigration = await MigrateRunner(runnerItem, newLocation, migrationStrategy, cancellationToken);
            
            if (resultMigration.IsFailure)
                return resultMigration;
            
            runnerItem.UpdatedAt = _timeProvider.GetUtcNow();
            _dbContext.RunnerItems.Update(runnerItem);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Success;
    }
    
    private async ValueTask<Result> MigrateRunner(RunnerItem runnerItem, string newLocation, MigrateRunnersStrategy migrationStrategy, CancellationToken cancellationToken)
    {
        if (runnerItem.FolderPath == newLocation)
            return Success;

        var shouldStartRunner = runnerItem.State == RunnerState.Started;
        
        await _manager.SetRunner(runnerItem, false, cancellationToken);

        switch (migrationStrategy)
        {
            case MigrateRunnersStrategy.Clean:
                shouldStartRunner = false;
                _fileSystemManager.DirectoryDelete(runnerItem.FolderPath, true);
                runnerItem.Deleted = true;
                break;
            case MigrateRunnersStrategy.Move:
                _fileSystemManager.DirectoryMove(runnerItem.FolderPath, newLocation);
                runnerItem.FolderPath = newLocation;
                break;
            case MigrateRunnersStrategy.Copy:
                _fileSystemManager.DirectoryCopy(runnerItem.FolderPath, newLocation);
                runnerItem.FolderPath = newLocation;
                break;
            case MigrateRunnersStrategy.Ignore:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(migrationStrategy), migrationStrategy, null);
        }
        
        if(shouldStartRunner)
            await _manager.SetRunner(runnerItem, true, cancellationToken);

        return Success;
    }

    
    public enum MigrateRunnersStrategy
    {
        [CommandEnumValue("ignore")]
        Ignore,
        [CommandEnumValue("copy")]
        Copy,
        [CommandEnumValue("move")]
        Move,
        [CommandEnumValue("clean")]
        Clean
    }
}
