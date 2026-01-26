using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Runners.Persistence;
using SlimeTools.Commander;

namespace Runners.Middlewares;

public sealed class ExecuteMigrations : IMiddleware
{
    private readonly IRunnersDbContext _dbContext;
    
    public ExecuteMigrations(IRunnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<Result> Handle(ICommandContext context, MiddlewareHandler next, CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        return await next(context, cancellationToken);
    }
}
