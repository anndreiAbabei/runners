using CliWrap;

namespace Runners.Extensions;

public static class CliExtensions
{
    extension(Cli)
    {
        public static Command Shell()
        {
            var cmd = Os.IsWindows
                          ? "cmd"
                          : "sh";

            return Cli.Wrap(cmd);
        }
    }
}
