using Microsoft.Extensions.Logging;

namespace FiLogger.Extensions;

public static class LogLevelExtensions
{
    public static void ThrowIfOutOfRange(this LogLevel level)
    {
        if (level is < LogLevel.Trace or > LogLevel.Critical)
        {
            throw new ArgumentOutOfRangeException(nameof(level), "Invalid log level specified.");
        }
    }
}