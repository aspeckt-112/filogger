using FiLogger.Integration.Tests.Fixtures;
using FiLogger.Options;

using Microsoft.Extensions.Logging;

namespace FiLogger.Integration.Tests.Tests;

public abstract class BaseTest
{
    private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "FiLogger", $"{Path.GetRandomFileName()}.log");

    protected BaseTest(FiLoggerOptions options, FiLoggerFixture fixture)
    {
        Provider = new FiLoggerProvider(_testFilePath, options, LogLevel.Debug);
        fixture.AddLogFile(_testFilePath);
    }

    protected FiLoggerProvider Provider { get; }

    protected async Task<string[]> ReadLogLines() => await File.ReadAllLinesAsync(_testFilePath);
}