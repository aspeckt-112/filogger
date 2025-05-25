using System.Text;
using System.Threading.Channels;

using FiLogger.Enums;
using FiLogger.Extensions;
using FiLogger.Options;

using Microsoft.Extensions.Logging;

namespace FiLogger;

public sealed class FiLoggerProvider : ILoggerProvider
{
    private readonly FiLoggerOptions _options;
    private readonly StreamWriter _writer;
    private readonly Channel<string> _channel;
    private readonly Task _processingTask;
    private readonly CancellationTokenSource _cts = new();

    private bool _disposed;

    public LogLevel MinLogLevel { get; }

    public FiLoggerProvider(
        string filePath,
        FiLoggerOptions options,
        LogLevel minLogLevel = LogLevel.Information)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);
        minLogLevel.ThrowIfOutOfRange();

        CreateLogFileIfNotExists(filePath);

        _options = options;
        MinLogLevel = minLogLevel;

        _writer = BuildStreamWriter(filePath);
        _channel = BuildChannel();

        _processingTask = options.FlushMethod switch
        {
            FlushMethod.Immediate => ProcessLogQueue(true),
            FlushMethod.Periodic => Task.WhenAll(
                ProcessLogQueue(false),
                FlushPeriodically()),
            _ => throw new ArgumentOutOfRangeException(
                nameof(options.FlushMethod),
                "Invalid flush method specified.")
        };

        return;

        StreamWriter BuildStreamWriter(string path)
        {
            return new StreamWriter(path, true, Encoding.UTF8)
            {
                AutoFlush = false
            };
        }

        Channel<string> BuildChannel()
        {
            return Channel.CreateUnbounded<string>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    AllowSynchronousContinuations = false
                });
        }
    }

    public ILogger CreateLogger(string categoryName) => new FiLogger(categoryName, this);

    public void Write(string message) => _channel.Writer.TryWrite(message);

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

    private void CreateLogFileIfNotExists(string filePath)
    {
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
    }

    private async Task ProcessLogQueue(bool immediateFlush)
    {
        try
        {
            await foreach (string message in _channel.Reader.ReadAllAsync(_cts.Token))
            {
                await _writer.WriteLineAsync(message);

                if (immediateFlush)
                {
                    await _writer.FlushAsync();
                }
            }

            await _writer.FlushAsync();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task FlushPeriodically()
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