namespace FiLogger.Integration.Tests.Fixtures;

/// <summary>
/// This fixture exists in order to manage the lifecycle of log files created during tests.
/// Then, they can be deleted after the test run that the fixture is scoped to - instead of disposing between each test.
/// </summary>
public class FiLoggerFixture : IDisposable
{
    private readonly List<string> _logFiles = [];
    
    public void AddLogFile(string filePath)
    {
        _logFiles.Add(filePath);
    }

    public void Dispose()
    {
        foreach (string file in _logFiles)
        {
            File.Delete(file);
        }
    }
}