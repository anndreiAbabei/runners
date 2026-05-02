using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Runners.Extensions;
using Runners.Services;

namespace Runners.Persistence;

public interface IRunnersDbContext
{
    DatabaseFacade Database { get; }
    
    DbSet<RunnerItem> RunnerItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public sealed class RunnersDbContext : DbContext, IRunnersDbContext
{
    private readonly IRuntimeInformationProvider _runtimeInformationProvider;
    private string ConnectionString => field ??= CreateDbConnectionString();

    public DbSet<RunnerItem> RunnerItems { get; set; }

    public RunnersDbContext(IRuntimeInformationProvider runtimeInformationProvider) 
    {
        _runtimeInformationProvider = runtimeInformationProvider;
    }

    public RunnersDbContext(IRuntimeInformationProvider runtimeInformationProvider, DbContextOptions options) 
        : base(options)
    {
        _runtimeInformationProvider = runtimeInformationProvider;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var entity = modelBuilder.Entity<RunnerItem>();
        
        entity.HasKey(i => i.Id);
        entity.Property(i => i.Id).UseAutoincrement();
        entity.Property(i => i.Name).HasMaxLength(100).IsRequired();
        entity.Property(i => i.GitUrl).HasMaxLength(200).IsRequired();
        entity.Property(i => i.FolderPath).HasMaxLength(200).IsRequired();
        entity.Property(i => i.CreatedAt).IsRequired();
        entity.Property(i => i.UpdatedAt);
        entity.Property(i => i.State).HasDefaultValue(RunnerState.Added);
        entity.Property(i => i.Tag).HasMaxLength(200);
        entity.Property(i => i.Deleted).HasDefaultValue(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        if(optionsBuilder.IsConfigured)
            return;
        
        optionsBuilder.UseSqlite(ConnectionString);
    }
    
    private string CreateDbConnectionString()
    {
        const string fileName = "data.db";

        var dataDir = _runtimeInformationProvider.GetStateDir();
        var filePath = Path.Combine(dataDir, fileName);
        
        var connectionString = new SqliteConnectionStringBuilder
        {
            BrowsableConnectionString = false,
            DataSource = filePath,
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        return connectionString.ToString();
    }
}
