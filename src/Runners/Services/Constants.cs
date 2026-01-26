using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;

namespace Runners.Services;

public sealed class Constants
{
    public const string AppName = "Runners";
    public const string SafeAppName = "runners_app";

    public static string GetRunnersFolder()
    {
        const string runnersFolder = "runners";
        var dataDir = GetStateDir();

        return Path.Combine(dataDir, runnersFolder);;
    }
    
    public static string CreateDbConnectionString()
    {
        const string fileName = "data.db";

        var dataDir = GetStateDir();
        var filePath = Path.Combine(dataDir, fileName);
        
        var connectionString = new SqliteConnectionStringBuilder
        {
            BrowsableConnectionString = false,
            DataSource = filePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
        };

        return connectionString.ToString();
    }
    
    private static string GetStateDir()
    {
        string folder;
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            folder = "/var/lib";
        
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            folder = "/var/db";

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            folder = "/Library/Application Support";

        // Windows: ProgramData for machine-wide state
        else
            folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        
#if DEBUG
        folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#endif

        folder = Path.Combine(folder, SafeAppName);
        Directory.CreateDirectory(folder);

        return folder;
    }
}
