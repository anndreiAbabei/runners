using CSharpFunctionalExtensions;

namespace Runners.Commands;

public sealed partial class ConfigureCommand
{
    private async ValueTask<Result> SetDbFilePath(string dbFilePath, CancellationToken cancellationToken)
    {
        var settings = await _appSettingsManager.Read(cancellationToken);
        
        settings.DbFilePath = dbFilePath;
        
        await _appSettingsManager.Write(settings, cancellationToken);

        return Success;
    }
}
