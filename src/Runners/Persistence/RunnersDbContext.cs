using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Runners.Services;

namespace Runners.Persistence;

public interface IRunnersDbContext
{
    DatabaseFacade Database { get; }
    
    DbSet<RunnerItem> RunnerItems { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public sealed class RunnersDbContext : DbContext, IRunnersDbContext
{
    internal static string ConnectionString => field ??= Constants.CreateDbConnectionString();
    
    public DbSet<RunnerItem> RunnerItems { get; set; }

    public RunnersDbContext() {}

    public RunnersDbContext(DbContextOptions options) 
        : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var entity = modelBuilder.Entity<RunnerItem>();
        entity.HasKey(i => i.Id);
        entity.Property(i => i.Id).UseAutoincrement();
        entity.Property(i => i.Name).HasMaxLength(100).IsRequired();
        entity.Property(i => i.GitUrl).HasMaxLength(100).IsRequired();
        entity.Property(i => i.FolderPath).HasMaxLength(100).IsRequired();
        entity.Property(i => i.CreatedAt).IsRequired();
        entity.Property(i => i.State).HasDefaultValue(RunnerState.Added);
        entity.Property(i => i.Tag).HasMaxLength(100);
        entity.Property(i => i.Deleted).HasDefaultValue(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        optionsBuilder.UseSqlite(ConnectionString);
    }
}
