using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Runners.Persistence;
using Runners.Services;
using SlimeTools.Commander;

namespace Runners.Commands;

public sealed class AddCommand : BaseCommand<AddCommand.AddCommandData>, ICommand<AddCommand.AddCommandData>
{
    private readonly IRunnersDbContext _dbContext;
    private readonly IRunnerManager _manager;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AddCommand> _logger;

    public AddCommand(IRunnersDbContext dbContext, 
                      IRunnerManager manager, 
                      TimeProvider timeProvider,
                      ILogger<AddCommand> logger)
    {
        _dbContext = dbContext;
        _manager = manager;
        _timeProvider = timeProvider;
        _logger = logger;
    }
    
    public static ICommandStructure CreateCommand(ICommandBuilder builder)
    {
        return builder.Create<AddCommandData>("add", new CommandDetails { Description = "Create a new GitHub runner" })
                      .WithFlag(d => d.GitHubUrl, "url", new FlagOptions { Description = "Url of the GitHub url", Optional = false })
                      .WithFlag(d => d.Token, "token", new FlagOptions { Description = "Token of the GitHub runner", Optional = false })
                      .WithFlag(d => d.Tag, "tag", new FlagOptions { Description = "Local name of the GitHub runner", Optional = true })
                      .WithFlag(d => d.NoDownload, "no-download", new FlagOptions { Description = "Flag to stop add before download (default: false)", Optional = true })
                      .WithFlag(d => d.NoConfig, "no-config", new FlagOptions { Description = "Flag to stop add before config (default: false)", Optional = true })
                      .WithFlag(d => d.NoInstall, "no-install", new FlagOptions { Description = "Flag to stop add before installing (default: false)", Optional = true })
                      .WithFlag(d => d.NoStart, "no-start", new FlagOptions { Description = "Flag to stop add before starting (default: false)", Optional = true })
                      .Build();
    }

    public async ValueTask<Result> Execute(ICommandContext context, CancellationToken cancellationToken)
    {
        var existingRunner = await _dbContext.RunnerItems.FirstOrDefaultAsync(i => i.GitUrl == Data.GitHubUrl && !i.Deleted,
                                                                              cancellationToken);

        if (existingRunner != null)
            return Failure($"There's already an existing runner for {existingRunner.GitUrl} on {existingRunner.FolderPath}. " +
                           $"Delete the existing one and try again.");

        var (_, isFailure, runner, error) = await CreateRunnerItem(cancellationToken);

        if (isFailure)
            return Failure(error);

        if (Data.NoDownload)
        {
            _logger.LogInformation($"{nameof(Data.NoDownload)} flag set to true, skipping installing {{RunnerId}}", runner.Id);
            return Success;
        }

        (_, isFailure, error) = await DownloadRunner(runner, cancellationToken);

        if (isFailure)
            return Failure(error);

        if (Data.NoConfig)
        {
            _logger.LogInformation($"{nameof(Data.NoConfig)} flag set to true, skipping config {{RunnerId}}", runner.Id);
            return Success;
        }

        (_, isFailure, error) = await ConfigRunner(runner, cancellationToken);

        if (isFailure)
            return Failure(error);

        if (Data.NoInstall)
        {
            _logger.LogInformation($"{nameof(Data.NoInstall)} flag set to true, skipping install {{RunnerId}}", runner.Id);
            return Success;
        }

        (_, isFailure, error) = await InstallRunner(runner, cancellationToken);

        if (isFailure)
            return Failure(error);

        if (Data.NoStart)
        {
            _logger.LogInformation($"{nameof(Data.NoStart)} flag set to true, skipping start {{RunnerId}}", runner.Id);
            return Success;
        }

        (_, isFailure, error) = await StartRunner(runner, cancellationToken);

        if (isFailure)
            return Failure(error);
        
        return Success;
    }

    private async ValueTask<Result<RunnerItem>> CreateRunnerItem(CancellationToken cancellationToken)
    {
        var (_, isFailure, runnerName, error) = _manager.CreateName(Data.GitHubUrl);

        if (isFailure)
            return Result.Failure<RunnerItem>(error);

        (_, isFailure, var folderPath, error) = _manager.GetRunnerFolder(runnerName);

        if (isFailure)
            return Result.Failure<RunnerItem>(error);
        
        var runner = new RunnerItem
        {
            Name = runnerName,
            GitUrl = Data.GitHubUrl,
            FolderPath = folderPath,
            CreatedAt = _timeProvider.GetUtcNow(),
            Tag = Data.Tag
        };
        await _dbContext.RunnerItems.AddAsync(runner, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Created runner item {RunnerId} in state {State}", runner.Id, runner.State);

        return runner;
    }
    
    private async ValueTask<Result> DownloadRunner(RunnerItem runner, CancellationToken cancellationToken)
    {
        var downloadResult = await _manager.DownloadRunner(runner, cancellationToken);

        if (downloadResult.IsFailure)
            return downloadResult;

        runner.State = RunnerState.Downloaded;
        _dbContext.RunnerItems.Update(runner);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Runner item {RunnerId} now state {State}", runner.Id, runner.State);

        return Result.Success();
    }
    
    private async ValueTask<Result> ConfigRunner(RunnerItem runner, CancellationToken cancellationToken)
    {
        var configureResult = await _manager.ConfigureRunner(runner, Data.Token, cancellationToken);

        if (configureResult.IsFailure)
            return configureResult;

        runner.State = RunnerState.Configured;
        _dbContext.RunnerItems.Update(runner);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Runner item {RunnerId} now state {State}", runner.Id, runner.State);

        return Result.Success();
    }
    
    private async ValueTask<Result> InstallRunner(RunnerItem runner, CancellationToken cancellationToken)
    {
        var installResult = await _manager.InstallRunner(runner, cancellationToken);

        if (installResult.IsFailure)
            return installResult;

        runner.State = RunnerState.Installed;
        _dbContext.RunnerItems.Update(runner);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Runner item {RunnerId} now state {State}", runner.Id, runner.State);

        return Result.Success();
    }
    
    private async ValueTask<Result> StartRunner(RunnerItem runner, CancellationToken cancellationToken)
    {
        var installResult = await _manager.SetRunner(runner, true, cancellationToken);

        if (installResult.IsFailure)
            return installResult;

        runner.State = RunnerState.Started;
        _dbContext.RunnerItems.Update(runner);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Runner item {RunnerId} now state {State}", runner.Id, runner.State);

        return Result.Success();
    }
    
    public class AddCommandData : ICommandData
    {
        public required string GitHubUrl { get; init; }
        
        public required string Token { get; init; }
        
        public string? Tag { get; init; }
        
        public bool NoDownload { get; init; }
        
        public bool NoConfig { get; init; }
        
        public bool NoInstall { get; init; }
        
        public bool NoStart { get; init; }
    }
}