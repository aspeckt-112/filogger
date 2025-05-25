using FiLogger.Enums;
using FiLogger.Integration.Tests.Fixtures;
using FiLogger.Options;

using Microsoft.Extensions.Logging;

namespace FiLogger.Integration.Tests.Tests;

public class PeriodicFlushTests : BaseTest, IClassFixture<FiLoggerFixture>
{
    private const int FlushIntervalSeconds = 3;
    public PeriodicFlushTests(FiLoggerFixture fixture) :
        base(FiLoggerOptions.CreatePeriodic(FlushIntervalSeconds), fixture)
    {
    }

    [Fact]
    public async Task PeriodicFlush_SingleLine_WillWriteToFile()
    {
        ILogger logger = Provider.CreateLogger(nameof(PeriodicFlushTests));

        // Act
        logger.LogInformation("This is a test log message.");

        // Assert
        string[] emptyLines = await ReadLogLines();
        Assert.Empty(emptyLines);
        await DelayForFlush();
        string[] lines = await ReadLogLines();
        Assert.Single(lines);
    }
    
    [Fact]
    public async Task PeriodicFlush_NothingWrittenInFlushInterval_NothingAdditionalWritten()
    {
        ILogger logger = Provider.CreateLogger(nameof(PeriodicFlushTests));
        
        logger.LogInformation("Something written before flush interval");
        
        await DelayForFlush();
        
        string[] linesBeforeFlush = await ReadLogLines();
        
        Assert.Single(linesBeforeFlush);
        
        // Wait for periodic flush without writing anything
        await DelayForFlush();
        
        // Assert that no new lines were written
        string[] linesAfterFlush = await ReadLogLines();
        Assert.Single(linesAfterFlush);
        Assert.Equal(linesBeforeFlush, linesAfterFlush);
    }
    
    private Task DelayForFlush() => Task.Delay(TimeSpan.FromSeconds(FlushIntervalSeconds * 2));
}