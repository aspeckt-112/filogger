using FiLogger.Integration.Tests.Fixtures;

using Microsoft.Extensions.Logging;

namespace FiLogger.Integration.Tests.Tests;

public class FiLoggerImmediateFlushTests : BaseFiLoggerTest, IClassFixture<FiLoggerFixture>
{
    public FiLoggerImmediateFlushTests(FiLoggerFixture fixture) :
        base(
            new FiLoggerOptions
            {
                FlushMethod = FlushMethod.Immediate
            },
            fixture)
    {
    }

    [Fact]
    public async Task Immediate_SingleLine_WillWriteToFile()
    {
        ILogger logger = Provider.CreateLogger(nameof(FiLoggerImmediateFlushTests));

        // Act
        logger.LogInformation("This is a test log message.");
        Provider.Dispose();

        // Assert
        string[] lines = await ReadLogLines();
        Assert.Single(lines);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    [InlineData(256)]
    [InlineData(512)]
    [InlineData(1024)]
    [InlineData(2048)]
    [InlineData(4096)]
    [InlineData(8192)]
    [InlineData(16384)]
    [InlineData(32768)]
    [InlineData(65536)]
    [InlineData(131072)]
    [InlineData(262144)]
    [InlineData(524288)]
    [InlineData(1048576)]
    [InlineData(2097152)]
    [InlineData(4194304)]
    [InlineData(8388608)]
    [InlineData(16777216)]
    public async Task Immediate_MultiLine_WillWriteToFile(int lineCount)
    {
        // Arrange
        ILogger logger = Provider.CreateLogger(nameof(FiLoggerImmediateFlushTests));

        // Act
        for (var i = 0; i < lineCount; i++)
        {
            logger.LogInformation($"This is test log message {i + 1}.");
        }

        Provider.Dispose();

        // Assert
        string[] lines = await ReadLogLines();
        Assert.Equal(lineCount, lines.Length);
    }
}