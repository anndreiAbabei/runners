using System.Runtime.InteropServices;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Runners.Extensions;
using Runners.Persistence;
using Runners.Services.Commands;

namespace Runners.Services;

public interface IRunnerManager
{
    Result<string> CreateName(string gitHubUrl);
    ValueTask<Result<string>> GetRunnerFolder(string runnerName, CancellationToken cancellationToken);
    ValueTask<Result<string>> GetState(RunnerItem runner, CancellationToken cancellationToken);
    ValueTask<Result> DownloadRunner(RunnerItem runner, CancellationToken cancellationToken, string? downloadUrl = null);
    ValueTask<Result> ConfigureRunner(RunnerItem runner, string token, CancellationToken cancellationToken);
    ValueTask<Result> InstallRunner(RunnerItem runner, CancellationToken cancellationToken);
    ValueTask<Result> SetRunner(RunnerItem runner, bool start, CancellationToken cancellationToken);
    ValueTask<Result> DeleteRunner(RunnerItem runner, CancellationToken cancellationToken);
}

public sealed class RunnerManager : IRunnerManager
{
    private readonly IRuntimeInformationProvider _runtimeInformation;
    private readonly ICommandProvider _commandProvider;
    private readonly IFileSystemManager _fileManager;
    private readonly IAppSettingsManager _appSettingsManager;
    private readonly ILogger<RunnerManager> _logger;
    
    internal const string DefaultRunnerDownloadVersion = "2.334.0";
    
    public RunnerManager(IRuntimeInformationProvider runtimeInformation, 
                         ICommandProvider commandProvider,
                         IFileSystemManager fileManager,
                         IAppSettingsManager appSettingsManager,
                         ILogger<RunnerManager> logger)
    {
        _runtimeInformation = runtimeInformation;
        _commandProvider = commandProvider;
        _fileManager = fileManager;
        _appSettingsManager = appSettingsManager;
        _logger = logger;
    }
    
    public Result<string> CreateName(string gitHubUrl)
    {
        var uri = new Uri(gitHubUrl);

        return uri.Segments.Length != 3 
                   ? Result.Failure<string>("Invalid format of GitHub url") 
                   : $"{uri.Segments[1]}_{uri.Segments[2]}".Replace("/", string.Empty);
    }

    public async ValueTask<Result<string>> GetRunnerFolder(string runnerName, CancellationToken cancellationToken)
    {
        var runnersFolder = await GetRunnersFolder(cancellationToken);
        var runnerFolder = Path.Combine(runnersFolder, runnerName);

        if (!_fileManager.DirectoryExists(runnerFolder))
        {
            _fileManager.DirectoryCreate(runnerFolder);
            _logger.LogDebug("Created directory {RunnerFolder} for runner {RunnerName}", runnerFolder, runnerName);
        }

        return Result.Success(runnerFolder);
    }
    
    public async ValueTask<Result<string>> GetState(RunnerItem runner, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting status (using sudo) for {Runner} with state {State}", runner.Id, runner.State);
        
        var extension = _runtimeInformation.GetShellExtension();
        var svcFile = Path.ChangeExtension("svc", extension);
        
        svcFile = Path.Combine(runner.FolderPath, svcFile);
        string[] args = ["-c", $"sudo {svcFile} status"];
        
        var result = await Run(_commandProvider.Shell(), args, runner.FolderPath, cancellationToken);
        
        if (result.IsFailure)
            return result;
        
        return result.Value.Contains("(running)") ? "Live" : "Stopped";
    }
    
    public async ValueTask<Result> DownloadRunner(RunnerItem runner, CancellationToken cancellationToken, string? downloadUrl = null)
    {
        var resultDwInfo = GetDownloadInfo(downloadUrl);
        
        if(resultDwInfo.IsFailure)
            return resultDwInfo;

        var (url, fileName) = resultDwInfo.Value; 
        
        var filePath = Path.Combine(runner.FolderPath, fileName);
            
        var dwCmd = _runtimeInformation.IsWindows
                        ? _commandProvider.Create("powershell")
                             .WithArguments(["Invoke-WebRequest", "-Uri", url, "-OutFile", fileName])
                        : _commandProvider.Create("curl")
                                          .WithArguments(["-o", fileName, "-L", url]);
        
        _logger.LogDebug("Downloading {Runner} to {FilePath}", url, filePath);

        var resultDw = await RunInShell(dwCmd, runner.FolderPath, cancellationToken);

        if (resultDw.IsFailure)
            return resultDw;
        
        _logger.LogDebug("Downloaded successfully to {FilePath}", filePath);
        
        var unzipCmd = _runtimeInformation.IsWindows
                           ? _commandProvider.Create("powershell")
                                             .WithArguments(
                                             [
                                                 "Add-Type", "-AssemblyName", "System.IO.Compression.FileSystem ;", 
                                                 $"[System.IO.Compression.ZipFile]::ExtractToDirectory(\"$PWD/{fileName}\", \"$PWD\")"
                                             ])
                           : _commandProvider.Create("tar")
                                             .WithArguments(["xzf", fileName]);
        
        _logger.LogDebug("Unzipping {FilePath}", filePath);

        var resultUnzip = await RunInShell(unzipCmd, runner.FolderPath, cancellationToken); 
        
        if (resultUnzip.IsFailure)
            return resultUnzip;
        
        _logger.LogDebug("Unzipped successfully file {FilePath}", filePath);

        return Result.Success();
        
    }

    public async ValueTask<Result> ConfigureRunner(RunnerItem runner, string token, CancellationToken cancellationToken)
    {
        var extension = _runtimeInformation.IsWindows ? "cmd" : "sh";
        var configFile = Path.ChangeExtension("config", extension);
        
        _logger.LogDebug("Executing {ConfigFile} for repo {RunnerRepo} (RunnerId: {RunnerId}) in {RunnerFolder}", 
                         configFile, runner.GitUrl, runner.Id, runner.FolderPath);
        string[] args = [configFile, "--url", runner.GitUrl, "--token", token];

        var result = await RunInShell(_commandProvider.Shell().WithArguments(args), runner.FolderPath, cancellationToken);

        return result;
    }
    
    public async ValueTask<Result> InstallRunner(RunnerItem runner, CancellationToken cancellationToken)
    {
        var extension = _runtimeInformation.IsWindows ? "cmd" : "sh";
        var svcFile = Path.ChangeExtension("svc", extension);
        
        _logger.LogDebug("Executing (as sudo) {ConfigFile} install for repo {RunnerRepo} (RunnerId: {RunnerId}) in {RunnerFolder}", 
                         svcFile, runner.GitUrl, runner.Id, runner.FolderPath);
        svcFile = Path.Combine(runner.FolderPath, svcFile);
        string[] args = ["-c", $"sudo {svcFile} install"];

        var result = await RunInShell(_commandProvider.Shell()
                                                      .WithArguments(args), 
                                      runner.FolderPath, 
                                      cancellationToken);

        return result;
    }
    
    public async ValueTask<Result> SetRunner(RunnerItem runner, bool start, CancellationToken cancellationToken)
    {
        var extension = _runtimeInformation.IsWindows ? "cmd" : "sh";
        var svcFile = Path.ChangeExtension("svc", extension);
        var state = start ? "start" : "stop";
        
        _logger.LogDebug("Executing (as sudo) {ConfigFile} {State} for repo {RunnerRepo} (RunnerId: {RunnerId}) in {RunnerFolder}", 
                         svcFile, state, runner.GitUrl, runner.Id, runner.FolderPath);
        svcFile = Path.Combine(runner.FolderPath, svcFile);
        string[] args = ["-c", $"sudo {svcFile} {state}"];

        var result = await RunInShell(_commandProvider.Shell().WithArguments(args), runner.FolderPath, cancellationToken);

        return result;
    }
    
    public async ValueTask<Result> DeleteRunner(RunnerItem runner, CancellationToken cancellationToken)
    {
        if (!_fileManager.DirectoryExists(runner.FolderPath))
            return Result.Success();
        
        _ = await SetRunner(runner, false, cancellationToken);
        
        var extension = _runtimeInformation.IsWindows ? "cmd" : "sh";
        var svcFile = Path.ChangeExtension("svc", extension);
        var configFile = Path.ChangeExtension("config", extension);
        
        _logger.LogDebug("Executing (as sudo) {SvcFile} uninstall for repo {RunnerRepo} (RunnerId: {RunnerId}) in {RunnerFolder}", 
                         svcFile, runner.GitUrl, runner.Id, runner.FolderPath);
        
        svcFile = Path.Combine(runner.FolderPath, svcFile);
        string[] args = ["-c", $"sudo {svcFile} uninstall"];
        var resultSvc = await RunInShell(_commandProvider.Shell().WithArguments(args), runner.FolderPath, cancellationToken);
        
        _logger.LogDebug("Executing {ConfigFile} remove for repo {RunnerRepo} (RunnerId: {RunnerId}) in {RunnerFolder}", 
                         configFile, runner.GitUrl, runner.Id, runner.FolderPath);
        
        args = [configFile, "remove"];
        var resultConfig = await RunInShell(_commandProvider.Shell().WithArguments(args), runner.FolderPath, cancellationToken);
        
        _fileManager.DirectoryDelete(runner.FolderPath, true);

        return Result.Combine(resultSvc, resultConfig);
    }
    
    private async ValueTask<Result<string>> Run(ICommand command, string[] args, string workingDirectory, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing command {Command} with {Arguments}, in {WorkingDirectory}", 
                         command.TargetFilePath, args, workingDirectory);
        
        var sb = new StringBuilder();
        var result = await command.WithArguments(args)
                                  .WithWorkingDirectory(workingDirectory)
                                  .Execute(cancellationToken);

        return result.IsFailure 
                   ? Result.Failure<string>($"{string.Join(' ', args)} failed with {result.Error}") 
                   : Result.Success(sb.ToString());
    }

    private async ValueTask<Result> RunInShell(ICommand command, string workingDirectory, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing command {Command} with {Arguments} (on {WorkingDirectory}) in shell.", 
                         command.TargetFilePath, command.Arguments, workingDirectory);
        
        var result = await command.WithWorkingDirectory(workingDirectory)
                                  .ExecuteInShell(cancellationToken);

        return result.IsFailure 
                   ? Result.Failure($"Fail to run {command.Arguments} with error {result.Error}") 
                   : Result.Success();
    }
    
    private async ValueTask<string> GetRunnersFolder(CancellationToken cancellationToken)
    {
        var appSettings = await _appSettingsManager.Read(cancellationToken);
        
        if(!string.IsNullOrEmpty(appSettings.RunnersFolder))
            return appSettings.RunnersFolder;
        
        const string runnersFolder = "runners";
        var dataDir = _runtimeInformation.GetStateDir(_fileManager);

        return Path.Combine(dataDir, runnersFolder);
    }
    
    private Result<DownloadInfo> GetDownloadInfo(string? downloadUrl)
    {
        var extension = _runtimeInformation.IsWindows ? "zip" : "tar.gz";

        var fileName = Path.ChangeExtension("actionRunner", extension);
        
        if(!string.IsNullOrEmpty(downloadUrl))
            return new DownloadInfo(downloadUrl, fileName);
        
        var arch = _runtimeInformation.ProcessArchitecture switch
        {
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm64",
            Architecture.X86 or Architecture.X64 => "x64",
            _ => null
        };
        
        string? os = null;
        if (_runtimeInformation.IsMacOs)
            os = "osx";
        else if (_runtimeInformation.IsWindows)
            os = "win";
        else if (_runtimeInformation.IsLinux)
            os = "linux";

        if (string.IsNullOrEmpty(arch))
            return Result.Failure<DownloadInfo>($"Invalid architecture (Only supported ARM/64 or x86/64), " +
                                                $"`{_runtimeInformation.ProcessArchitecture}` is not supported. Get a real processor.");
        if (string.IsNullOrEmpty(os))
            return Result.Failure<DownloadInfo>("Invalid OS (Only supported osx, win, linux). get a real OS!");

        var url = $"https://github.com/actions/runner/releases/download/v{DefaultRunnerDownloadVersion}/actions-runner-{os}-{arch}-{DefaultRunnerDownloadVersion}.{extension}";
        
        return new DownloadInfo(fileName, url);
    }

    private sealed record DownloadInfo(string Url, string FileName);
}
