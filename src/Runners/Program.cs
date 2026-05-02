using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Runners.Commands;
using Runners.Logging;
using Runners.Middlewares;
using Runners.Persistence;
using Runners.Services;
using Runners.Services.Commands;
using Serilog;
using SlimeTools.Commander;

namespace Runners;

public class Program
{
    public static async Task Main(string[] args)
    {
#if DEBUG
        // args = ["add", "--url", "https://github.com/org/repo", "--token", "test"]
        // args = ["list"]
        // args = ["set", "1", "stop"]
        // args = ["set", "1", "start"]
        // args = ["delete", "1", "--yes"]
        args = ["help"]
        ;
#endif
        var sp = CreateServices(args);

        var logger = sp.GetRequiredService<ILogger<Program>>();
        var executor = sp.GetExecutor();

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, _) => cts.Cancel();
        var ct = cts.Token;

        var result = await executor.Run(ct);

        if (result.IsFailure)
        {
            logger.LogCritical("Execution failed with a failure: {Error}", result.Error);
            Environment.Exit(1);
        }
        else
            logger.LogDebug("Command finished.");
    }
    
    private static ServiceProvider CreateServices(string[] args)
    {
        var sc = new ServiceCollection();

        sc.AddSingleton<IRunnerManager, RunnerManager>()
          .AddSingleton<IVersionProvider, VersionProvider>()
          .AddSingleton<IRuntimeInformationProvider, RuntimeInformationProvider>()
          .AddSingleton<ICommandProvider, CommandProvider>()
          .AddSingleton<IFileSystemManager, FileSystemManager>()
          .AddSingleton<IAppSettingsManager, AppSettingsManager>()
          .AddSingleton<IConsolePrint, ConsolePrintWithLogging>()
          .AddSingleton(TimeProvider.System);

        sc.AddCommands(args, builder =>
        {
            builder.AddMiddleware<ExecuteMigrations>();
        
            builder.AddCommand<ListCommand>();
            builder.AddCommand<AddCommand>();
            builder.AddCommand<SetCommand>();
            builder.AddCommand<DeleteCommand>();
            builder.AddCommand<VersionCommand>();
        });

        var logFile = Path.Combine("logs", $"log{DateTime.UtcNow:yyyy-MM-ddTHHmmss}.log");
        Directory.CreateDirectory(Path.GetDirectoryName(logFile)!);
        
        var c = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true)
                                          .Build();
        
        Log.Logger = new LoggerConfiguration()
                     .ReadFrom.Configuration(c)
                     .CreateLogger();

        sc.AddLogging(lb => lb.AddSerilog());

        sc.AddDbContext<IRunnersDbContext, RunnersDbContext>(); 

        return sc.BuildServiceProvider();
    }
}
