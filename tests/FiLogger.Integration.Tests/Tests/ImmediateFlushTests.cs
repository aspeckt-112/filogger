using System.Collections;

using FiLogger.Integration.Tests.Fixtures;
using FiLogger.Options;

using Microsoft.Extensions.Logging;

namespace FiLogger.Integration.Tests.Tests;

public class ImmediateFlushTests : BaseTest, IClassFixture<FiLoggerFixture>
{
    private class LineCountData : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new()
        {
            new object[] { 2 },
            new object[] { 4 },
            new object[] { 8 },
            new object[] { 16 },
            new object[] { 32 },
            new object[] { 64 },
            new object[] { 128 },
            new object[] { 256 },
            new object[] { 512 },
            new object[] { 1024 },
            new object[] { 2048 },
            new object[] { 4096 },
            new object[] { 8192 },
            new object[] { 16384 },
            new object[] { 32768 },
            new object[] { 65536 },
            new object[] { 131072 },
            new object[] { 262144 },
            new object[] { 524288 },
            new object[] { 1048576 },
            new object[] { 2097152 },
            new object[] { 4194304 },
            new object[] { 8388608 },
            new object[] { 16777216 }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public ImmediateFlushTests(FiLoggerFixture fixture) :
        base(FiLoggerOptions.CreateImmediate(), fixture)
    {
    }

    [Fact]
    public async Task Immediate_SingleLine_WillWriteToFile()
    {
        ILogger logger = Provider.CreateLogger(nameof(ImmediateFlushTests));

        // Act
        logger.LogInformation("This is a test log message.");
        Provider.Dispose();

        // Assert
        string[] lines = await ReadLogLines();
        Assert.Single(lines);
    }

    [Theory]
    [ClassData(typeof(LineCountData))]
    public async Task Immediate_MultiLine_WillWriteToFile(int lineCount)
    {
        // Arrange
        ILogger logger = Provider.CreateLogger(nameof(ImmediateFlushTests));

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
    
    [Theory]
    [ClassData(typeof(LineCountData))]
    public async Task Immediate_MultiLine_WritesOrdered(int lineCount)
    {
        // Arrange
        ILogger logger = Provider.CreateLogger(nameof(ImmediateFlushTests));

        // Act
        for (var i = 0; i < lineCount; i++)
        {
            logger.LogInformation($"This is test log message {i + 1}.");
        }

        Provider.Dispose();

        // Assert
        string[] lines = await ReadLogLines();
        for (int i = 0; i < lineCount; i++)
        {
            Assert.Contains($"This is test log message {i + 1}.", lines[i]);
        }
    }
    
    [Theory]
    [ClassData(typeof(LineCountData))]
    public async Task Immediate_ConcurrentWrites_WillWriteToFile(int lineCount)
    {
        // Arrange
        ILogger logger = Provider.CreateLogger(nameof(ImmediateFlushTests));

        var tasks = new List<Task>();

        // Act
        for (var i = 0; i < lineCount; i++)
        {
            int index = i; // Capture the current value of i
            tasks.Add(Task.Run(() => logger.LogInformation($"This is test log message {index + 1}.")));
        }

        await Task.WhenAll(tasks);
        Provider.Dispose();

        // Assert
        string[] lines = await ReadLogLines();
        Assert.Equal(lineCount, lines.Length); // Can't assume order in concurrent writes - just check count. User should handle ordering outside of logger if needed.
    }
}