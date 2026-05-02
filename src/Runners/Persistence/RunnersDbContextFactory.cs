using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging.Abstractions;
using Runners.Services;

namespace Runners.Persistence;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Used for ef add migration")]
public sealed class RunnersDbContextFactory : IDesignTimeDbContextFactory<RunnersDbContext>
{
    public RunnersDbContext CreateDbContext(string[] args)
    {
        var provider = new RuntimeInformationProvider();
        var fileManager = new FileSystemManager(new NullLogger<FileSystemManager>());
        var appSettingsManager = new AppSettingsManager(fileManager);
        var log = new NullLogger<RunnersDbContext>();
        
        return new RunnersDbContext(provider, fileManager, appSettingsManager, log);
    }
}
