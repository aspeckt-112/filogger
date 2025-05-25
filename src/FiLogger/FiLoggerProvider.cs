using System.Text;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

namespace FiLogger;

public class FiLoggerProvider : ILoggerProvider
{
    private readonly FiLoggerOptions _options;
    private readonly Channel<string> _channel;
    private readonly Task _processingTask;
    private readonly StreamWriter _writer;
    private readonly CancellationTokenSource _cts = new();
    private bool _disposed;

    public LogLevel MinLogLevel { get; }

    public FiLoggerProvider(
        string filePath,
        FiLoggerOptions options,
        LogLevel minLogLevel = LogLevel.Information)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        if (minLogLevel is < LogLevel.Trace or > LogLevel.Critical)
        {
            throw new ArgumentOutOfRangeException(nameof(minLogLevel), "Invalid log level specified.");
        }

        if (!File.Exists(filePath))
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? string.Empty);
                using FileStream _ = File.Create(filePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to create log file at {filePath}.", ex);
            }
        }

        if (options.FlushMethod is FlushMethod.Periodic && options.FlushIntervalSeconds is null or <= 0)
        {
            throw new InvalidOperationException(
                "The flush interval must be specified and greater than 0 when using Periodic flush method.");
        }

        _options = options;

        MinLogLevel = minLogLevel;

        _writer = new StreamWriter(filePath, true, Encoding.UTF8)
        {
            AutoFlush = false
        };

        _channel = Channel.CreateUnbounded<string>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                AllowSynchronousContinuations = false
            });

        _processingTask = options.FlushMethod switch
        {
            FlushMethod.Immediate => Task.Run(ProcessLogQueueImmediateAsync),
            FlushMethod.Periodic => Task.WhenAll(
                Task.Run(ProcessLogQueuePeriodicAsync),
                Task.Run(FlushPeriodicallyAsync)),
            _ => throw new ArgumentOutOfRangeException() // Fix this
        };
    }

    public ILogger CreateLogger(string categoryName) => new FiLoggerImmediateFlush(categoryName, this);

    public bool TryWrite(string message) => _channel.Writer.TryWrite(message);
    
    private async Task ProcessLogQueueImmediateAsync()
    {
        try
        {
            await foreach (string message in _channel.Reader.ReadAllAsync(_cts.Token))
            {
                await _writer.WriteLineAsync(message);
                await _writer.FlushAsync();
            }

            await _writer.FlushAsync();
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation gracefully
        }
    }

    private async Task ProcessLogQueuePeriodicAsync()
    {
        try
        {
            await foreach (string message in _channel.Reader.ReadAllAsync(_cts.Token))
            {
                await _writer.WriteLineAsync(message);
            }

            await _writer.FlushAsync();
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _cts.Cancel();
        _channel.Writer.Complete();

        _processingTask.Wait();
        _writer.Dispose();
    }

    private async Task FlushPeriodicallyAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(_options.FlushIntervalSeconds!.Value),
                    _cts.Token);

                await _writer.FlushAsync();
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}