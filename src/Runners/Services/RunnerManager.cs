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
    Result<string> GetRunnerFolder(string runnerName);
    ValueTask<Result<string>> GetState(RunnerItem runner, CancellationToken token);
    ValueTask<Result> DownloadRunner(RunnerItem runner, CancellationToken cancellationToken);
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
    private readonly ILogger<RunnerManager> _logger;
    
    public RunnerManager(IRuntimeInformationProvider runtimeInformation, 
                         ICommandProvider commandProvider,
                         IFileSystemManager fileManager,
                         ILogger<RunnerManager> logger)
    {
        _runtimeInformation = runtimeInformation;
        _commandProvider = commandProvider;
        _fileManager = fileManager;
        _logger = logger;
    }
    
    public Result<string> CreateName(string gitHubUrl)
    {
        var uri = new Uri(gitHubUrl);

        return uri.Segments.Length != 3 
                   ? Result.Failure<string>("Invalid format of GitHub url") 
                   : $"{uri.Segments[1]}_{uri.Segments[2]}".Replace("/", string.Empty);
    }

    public Result<string> GetRunnerFolder(string runnerName)
    {
        var runnersFolder = Constants.GetRunnersFolder();
        var runnerFolder = Path.Combine(runnersFolder, runnerName);

        if (!_fileManager.DirectoryExists(runnerFolder))
        {
            _fileManager.DirectoryCreate(runnerFolder);
            _logger.LogDebug("Created directory {RunnerFolder} for runner {RunnerName}", runnerFolder, runnerName);
        }

        return Result.Success(runnerFolder);
    }
    
    public async ValueTask<Result<string>> GetState(RunnerItem runner, CancellationToken token)
    {
        _logger.LogDebug("Getting status for {Runner} with state {State}", runner.Id, runner.State);
        
        var extension = _runtimeInformation.GetShellExtension();
        var fileToCheck = Path.ChangeExtension("svc", extension);
        
        var result = await RunFile(fileToCheck, ["status"], runner.FolderPath, token);
        
        if (result.IsFailure)
            return result;
        
        return result.Value.Contains("Started") ? "Live" : "Stopped";
    }
    
    public async ValueTask<Result> DownloadRunner(RunnerItem runner, CancellationToken cancellationToken)
    {
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
            return Result.Failure($"Invalid architecture (Only supported ARM/64 or x86/64), " +
                                  $"`{_runtimeInformation.ProcessArchitecture}` is not supported. Get a real processor.");
        if (string.IsNullOrEmpty(os))
            return Result.Failure("Invalid OS (Only supported osx, win, linux). get a real OS!");

        var extension = _runtimeInformation.IsWindows ? "zip" : "tar.gz";

        var url = $"https://github.com/actions/runner/releases/download/v2.331.0/actions-runner-{os}-{arch}-2.331.0.{extension}";
        var fileName = Path.ChangeExtension("actionRunner", extension);
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
        
        _logger.LogDebug("Executing {ConfigFile} install for repo {RunnerRepo} (RunnerId: {RunnerId}) in {RunnerFolder}", 
                         svcFile, runner.GitUrl, runner.Id, runner.FolderPath);
        string[] args = [svcFile, "install"];

        var result = await RunInShell(_commandProvider.Shell().WithArguments(args), runner.FolderPath, cancellationToken);

        return result;
    }
    
    public async ValueTask<Result> SetRunner(RunnerItem runner, bool start, CancellationToken cancellationToken)
    {
        var extension = _runtimeInformation.IsWindows ? "cmd" : "sh";
        var svcFile = Path.ChangeExtension("svc", extension);
        var state = start ? "start" : "stop";
        
        _logger.LogDebug("Executing {ConfigFile} {State} for repo {RunnerRepo} (RunnerId: {RunnerId}) in {RunnerFolder}", 
                         svcFile, state, runner.GitUrl, runner.Id, runner.FolderPath);
        string[] args = [svcFile, state];

        var result = await RunInShell(_commandProvider.Shell().WithArguments(args), runner.FolderPath, cancellationToken);

        return result;
    }
    
    public async ValueTask<Result> DeleteRunner(RunnerItem runner, CancellationToken cancellationToken)
    {
        _ = await SetRunner(runner, false, cancellationToken);
        
        var extension = _runtimeInformation.IsWindows ? "cmd" : "sh";
        var svcFile = Path.ChangeExtension("svc", extension);
        var configFile = Path.ChangeExtension("config", extension);
        
        string[] args = [svcFile, "uninstall"];
        var resultSvc = await RunInShell(_commandProvider.Shell().WithArguments(args), runner.FolderPath, cancellationToken);
        
        args = [configFile, "remove"];
        var resultConfig = await RunInShell(_commandProvider.Shell().WithArguments(args), runner.FolderPath, cancellationToken);
        
        _fileManager.DirectoryDelete(runner.FolderPath, true);

        return Result.Combine(resultSvc, resultConfig);
    }
    
    private ValueTask<Result<string>> Execute(string command, string[] args, string workingDirectory, CancellationToken cancellationToken)
    {
        return Run(_commandProvider.Create(command), args, workingDirectory, cancellationToken);
    }
    
    private ValueTask<Result<string>> RunFile(string file, string[] args, string workingDirectory, CancellationToken cancellationToken)
    {
        return Run(_commandProvider.Shell(), [file, ..args], workingDirectory, cancellationToken);
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
                                  .Execute(cancellationToken);

        return result.IsFailure 
                   ? Result.Failure($"Fail to run {command.Arguments} with error {result.Error}") 
                   : Result.Success();
    }
}
