using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
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
    private readonly IFileSystemManager _fileSystemManager;
    private readonly IAppSettingsManager _appSettingsManager;
    private readonly ILogger<RunnersDbContext> _logger;
    private string ConnectionString => field ??= CreateDbConnectionString();

    public DbSet<RunnerItem> RunnerItems { get; set; }

    public RunnersDbContext(IRuntimeInformationProvider runtimeInformationProvider,
                            IFileSystemManager fileSystemManager,
                            IAppSettingsManager appSettingsManager,
                            ILogger<RunnersDbContext> logger)
    {
        _runtimeInformationProvider = runtimeInformationProvider;
        _fileSystemManager = fileSystemManager;
        _appSettingsManager = appSettingsManager;
        _logger = logger;
    }

    public RunnersDbContext(IRuntimeInformationProvider runtimeInformationProvider, DbContextOptions options,
                            IFileSystemManager fileSystemManager,
                            IAppSettingsManager appSettingsManager,
                            ILogger<RunnersDbContext> logger) 
        : base(options)
    {
        _runtimeInformationProvider = runtimeInformationProvider;
        _fileSystemManager = fileSystemManager;
        _appSettingsManager = appSettingsManager;
        _logger = logger;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var entity = modelBuilder.Entity<RunnerItem>();
        
        entity.HasKey(i => i.Id);
        entity.Property(i => i.Id).UseAutoincrement();
        entity.Property(i => i.Name).HasMaxLength(100).IsRequired();
        entity.Property(i => i.GitUrl).HasMaxLength(200).IsRequired();
        entity.Property(i => i.Token).HasMaxLength(200).IsRequired();
        entity.Property(i => i.FolderPath).HasMaxLength(200).IsRequired();
        entity.Property(i => i.CreatedAt).IsRequired();
        entity.Property(i => i.UpdatedAt);
        entity.Property(i => i.State).HasDefaultValue(RunnerState.Added);
        entity.Property(i => i.Tag).HasMaxLength(200);
        entity.Property(i => i.Deleted).HasDefaultValue(false);
        
        _logger.LogDebug("Configure model for EF DB");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        if(optionsBuilder.IsConfigured)
            return;
        
        optionsBuilder.UseSqlite(ConnectionString);
        
        _logger.LogDebug("Configuring DB to use {ConnectionString}", ConnectionString);
    }
    
    private string CreateDbConnectionString()
    {
        var filePath = _appSettingsManager.Read().AsTask()
                                          .GetAwaiter().GetResult()
                                          .DbFilePath;
            
        if(string.IsNullOrEmpty(filePath))
        {
            const string fileName = "data.db";
            var dataDir = _runtimeInformationProvider.GetStateDir(_fileSystemManager);
            filePath = Path.Combine(dataDir, fileName);
        }
        
        var connectionString = new SqliteConnectionStringBuilder
        {
            BrowsableConnectionString = false,
            DataSource = filePath,
            Mode = SqliteOpenMode.ReadWriteCreate
        };

        return connectionString.ToString();
    }
}
