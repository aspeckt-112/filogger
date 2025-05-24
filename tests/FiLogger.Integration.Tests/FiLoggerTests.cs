using Microsoft.Extensions.Logging;

namespace FiLogger.Integration.Tests;

public class FiLoggerTests : IDisposable
{
    private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "FiLogger", "FiLoggerTest.log");
    
    [Fact]
    public async Task Immediate_SingleLine_WillWriteToFile()
    {
        // Arrange
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFiLogger(
                _testFilePath,
                new FiLoggerOptions
                {
                    FlushMethod = FlushMethod.Immediate
                },
                LogLevel.Debug);
        });

        var logger = loggerFactory.CreateLogger<FiLoggerTests>();

        // Act
        logger.LogInformation("This is a test log message.");
        await Task.Delay(1); // Small delay to ensure the log is written

        // Assert
        var lines = await File.ReadAllLinesAsync(_testFilePath);
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
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFiLogger(
                _testFilePath,
                new FiLoggerOptions
                {
                    FlushMethod = FlushMethod.Immediate
                },
                LogLevel.Debug);
        });

        var logger = loggerFactory.CreateLogger<FiLoggerTests>();

        // Act
        for (int i = 0; i < lineCount; i++)
        {
            logger.LogInformation($"This is test log message {i + 1}.");
        }

        // Assert
        await Task.Delay(TimeSpan.FromSeconds(5)); // Small delay to ensure the logs are written
        var lines = await File.ReadAllLinesAsync(_testFilePath);
        Assert.Equal(lineCount, lines.Length);
    }
    
    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }
}