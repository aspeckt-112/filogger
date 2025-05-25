using FiLogger.Enums;

namespace FiLogger.Options;

public record FiLoggerOptions
{
    private FiLoggerOptions()
    {
    }
    
    public required FlushMethod FlushMethod { get; init; }
    
    public int? FlushIntervalSeconds { get; init; }

    public static FiLoggerOptions CreateImmediate()
    {
        return new FiLoggerOptions
        {
            FlushMethod = FlushMethod.Immediate
        };
    }
    
    public static FiLoggerOptions CreatePeriodic(int flushIntervalSeconds)
    {
        if (flushIntervalSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(flushIntervalSeconds), "Flush interval must be greater than 0.");
        }
        
        return new FiLoggerOptions
        {
            FlushMethod = FlushMethod.Periodic,
            FlushIntervalSeconds = flushIntervalSeconds
        };
    }
}