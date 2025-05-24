namespace FiLogger;

public record FiLoggerOptions
{
    public required FlushMethod FlushMethod { get; init; }
    
    public int? FlushIntervalSeconds { get; init; }
}