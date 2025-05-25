using FiLogger.Integration.Tests.Fixtures;

using Microsoft.Extensions.Logging;

namespace FiLogger.Integration.Tests.Tests;

public abstract class BaseFiLoggerTest : IDisposable
{
    private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "FiLogger", $"{Path.GetRandomFileName()}.log");

    protected BaseFiLoggerTest(FiLoggerOptions options, FiLoggerFixture fixture)
    {
        Provider = new FiLoggerProvider(_testFilePath, options, LogLevel.Debug);
        fixture.AddLogFile(_testFilePath);
    }

    protected FiLoggerProvider Provider { get; }

    protected async Task<string[]> ReadLogLines() => await File.ReadAllLinesAsync(_testFilePath);

    public void Dispose()
    {
        Provider.Dispose();
    }
}