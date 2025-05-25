using FiLogger.Integration.Tests.Fixtures;

using Microsoft.Extensions.Logging;

namespace FiLogger.Integration.Tests.Tests;

public class FiLoggerPeriodicFlushTests : BaseFiLoggerTest, IClassFixture<FiLoggerFixture>
{
    public FiLoggerPeriodicFlushTests(FiLoggerFixture fixture) :
        base(
            new FiLoggerOptions
            {
                FlushMethod = FlushMethod.Periodic,
                FlushIntervalSeconds = 5
            },
            fixture)
    {
    }

    [Fact]
    public async Task PeriodicFlush_SingleLine_WillWriteToFile()
    {
        ILogger logger = Provider.CreateLogger(nameof(FiLoggerPeriodicFlushTests));

        // Act
        logger.LogInformation("This is a test log message.");

        // Assert
        string[] emptyLines = await ReadLogLines();
        Assert.Empty(emptyLines);
        await Task.Delay(TimeSpan.FromSeconds(6)); // Wait for periodic flush
        string[] lines = await ReadLogLines();
        Assert.Single(lines);
    }
}