using Microsoft.Extensions.Logging;

namespace FiLogger;

public class FiLoggerImmediateFlush : ILogger
{
    private readonly string _category;
    private readonly FiLoggerProvider _provider;

    public FiLoggerImmediateFlush(string category, FiLoggerProvider provider)
    {
        _category = category;
        _provider = provider;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _provider.MinLogLevel;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = formatter(state, exception);

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        var logLine = $"{DateTime.UtcNow:o} [{logLevel}] {_category}: {message}";

        if (exception != null)
        {
            logLine += Environment.NewLine + exception;
        }

        _provider.TryWrite(logLine);
    }
}