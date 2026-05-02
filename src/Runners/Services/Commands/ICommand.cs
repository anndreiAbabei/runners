using System.Diagnostics;
using CSharpFunctionalExtensions;

namespace Runners.Services.Commands;

public interface ICommand
{
    string TargetFilePath { get; }
    string WorkingDirectory { get; }
    IEnumerable<string> Arguments { get; }
    
    ICommand WithArguments(IEnumerable<string> args);
    ICommand WithWorkingDirectory(string workingDirectory);
    ValueTask<Result<string>> Execute(CancellationToken cancellationToken);
    ValueTask<Result> ExecuteInShell(CancellationToken cancellationToken);
}

public sealed class CliCommand : ICommand
{
    private IEnumerable<string> _arguments = [];

    public string TargetFilePath { get; }
    public string WorkingDirectory { get; private set; }
    public IEnumerable<string> Arguments    
    {
        get => _arguments;
        init => _arguments = value;
    }

    public CliCommand(string command, IEnumerable<string>? args = null)
    {
        TargetFilePath = command;
        Arguments = args ?? [];
        WorkingDirectory = Environment.CurrentDirectory;
    }

    public ICommand WithArguments(IEnumerable<string> args)
    {
        _arguments = args;
        
        return this;    
    }
    public ICommand WithWorkingDirectory(string workingDirectory)
    {
        WorkingDirectory = workingDirectory;
        
        return this;
    }

    public async ValueTask<Result<string>> Execute(CancellationToken cancellationToken)
    {
        var pi = CreateProcessInfo(true);
        using var p = Process.Start(pi);

        if(p == null)
            return Result.Failure<string>($"Failed to invoke `{pi.FileName}` on `{pi.WorkingDirectory}`");

        var processTask = p.WaitForExitAsync(cancellationToken);
        var outputTask = p.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = p.StandardError.ReadToEndAsync(cancellationToken);
        await Task.WhenAll(processTask, outputTask, errorTask);
        
        return p.ExitCode != 0 
                   ? Result.Failure<string>(await errorTask) 
                   : Result.Success(await outputTask);
    }
    
    public async ValueTask<Result> ExecuteInShell(CancellationToken cancellationToken)
    {
        var pi = CreateProcessInfo(false);
        using var p = Process.Start(pi);

        if(p == null)
            return Result.Failure($"Failed to invoke `{TargetFilePath}` on `{WorkingDirectory}`");

        await p.WaitForExitAsync(cancellationToken);
        
        return p.ExitCode != 0 
                   ? Result.Failure($"Failed to invoke `{TargetFilePath}`. Error code: {p.ExitCode}") 
                   : Result.Success();
    }

    private ProcessStartInfo CreateProcessInfo(bool redirectOutput)
    {
        return new ProcessStartInfo(TargetFilePath, Arguments)
        {
            WorkingDirectory =  WorkingDirectory,
            RedirectStandardOutput = redirectOutput,
            RedirectStandardError = redirectOutput,
            UseShellExecute = !redirectOutput
        };
    }
}