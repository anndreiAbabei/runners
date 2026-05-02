using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Design;
using Runners.Services;

namespace Runners.Persistence;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Used for ef add migration")]
public sealed class RunnersDbContextFactory : IDesignTimeDbContextFactory<RunnersDbContext>
{
    public RunnersDbContext CreateDbContext(string[] args)
    {
        var provider = new RuntimeInformationProvider();
        
        return new RunnersDbContext(provider);
    }
}
