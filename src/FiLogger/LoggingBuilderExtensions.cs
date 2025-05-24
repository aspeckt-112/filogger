using Microsoft.Extensions.Logging;

namespace FiLogger;

public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds the FiLogger provider to the logging builder.
    /// </summary>
    /// <param name="builder">The logging builder.</param>
    /// <param name="filePath">The file path where logs will be written.</param>
    /// <param name="options">The options for configuring the FiLogger.</param>
    /// <param name="minLogLevel">The minimum log level to log.</param>
    /// <returns>The updated logging builder.</returns>
    public static ILoggingBuilder AddFiLogger(
        this ILoggingBuilder builder,
        string filePath,
        FiLoggerOptions options,
        LogLevel minLogLevel = LogLevel.Information)
    {
        builder.AddProvider(new FiLoggerProvider(filePath, options, minLogLevel));

        return builder;
    }
}